using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Febucci.UI;
using SO;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Infrastructure.Factories;
using VillageGame.Services;
using VillageGame.Services.Presents;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;
using VillageGame.UI.Panels.Warnings;
using Web;
using Web.ResponseStructs.PayloadValues;
using Object = UnityEngine.Object;

namespace VillageGame.UI.Controllers
{
    public class CalendarPanelController : UIPanelController
    {
        private readonly MainUIPanel _mainPanel;
        private readonly CalendarPanel _calendarPanel;

        private readonly CutSceneConfig _cutSceneConfig;
        private readonly ApplicationObserver _applicationObserver;
        private readonly TapPresentPanel _tapPresentPanel;
        private readonly CalendarPresentsStorage _calendarPresentStorage;
        private readonly UIFactory _uiFactory;
        private readonly TimeObserver _timeObserver;
        private readonly DodoBirdsGiftsConfig _config;
        private readonly CalendarShopButton _leftShopButtonPrefab;
        private readonly CalendarShopButton _rightShopButtonPrefab;
        private readonly PresentInformationPanel _presentInformationPanel;

        public Action<CalendarPresentButton> ShowPresentInfo;

        private CalendarPresentButton _currentTapPresent;

        [Inject]
        public CalendarPanelController(IObjectResolver objectResolver)
        {
            var uiFacade = objectResolver.Resolve<UIFacade>();
            _mainPanel = uiFacade.FindPanel<MainUIPanel>();
            _calendarPanel = uiFacade.FindPanel<CalendarPanel>();
            _tapPresentPanel = uiFacade.FindPanel<TapPresentPanel>();
            _calendarPresentStorage = objectResolver.Resolve<CalendarPresentsStorage>();
            _applicationObserver = objectResolver.Resolve<ApplicationObserver>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            _uiFactory = objectResolver.Resolve<UIFactory>();
            _cutSceneConfig = objectResolver.Resolve<CutSceneConfig>();
            _config = objectResolver.Resolve<DodoBirdsGiftsConfig>();
            _presentInformationPanel = uiFacade.FindPanel<PresentInformationPanel>();

            _leftShopButtonPrefab = uiFacade.LeftShopButton;
            _rightShopButtonPrefab = uiFacade.RightShopButton;

            SubscribeToEvents(true);
        }

        ~CalendarPanelController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _mainPanel.CalendarButton.ClickEvent += OnSwitchCalendar;
                _calendarPresentStorage.TakeNewClosedPresentBoxEvent += TakeNewPresentEvent;
                _calendarPresentStorage.OpenCalendarPresentEvent += OnOpenCalendarPresent;
                _applicationObserver.SceneLoadEvent += SceneLoad;

                DodoBirdsService.BalanceUpdated += ReloadBalance;
                _timeObserver.StartNewDayInGameEvent +=
                    () => _calendarPanel.RefreshPresentButtons(_timeObserver.CurrentDay);
                
            }
            else
            {
                _mainPanel.CalendarButton.ClickEvent -= OnSwitchCalendar;
                _calendarPresentStorage.TakeNewClosedPresentBoxEvent -= TakeNewPresentEvent;
                _calendarPresentStorage.OpenCalendarPresentEvent -= OnOpenCalendarPresent;
                _applicationObserver.SceneLoadEvent -= SceneLoad;

                DodoBirdsService.BalanceUpdated -= ReloadBalance;
            }
        }


        private void OnOpenCalendarPresent(List<OpenedPresentBoxData> _, PresentBoxData presentBoxData)
        {
            var button = _calendarPanel.Buttons
                .FirstOrDefault(b => b.Data.OpenDate.Equals(presentBoxData.OpenDate));
            if (button != null)
            {
                button.SetOpenedMode();
            }
        }

        private void ReloadBalance()
        {
            _calendarPanel.LoadDodoBirdCounter(DodoBirdsService.Balance);
        }

        private void ReloadShop()
        {
            var bought = new List<DodoBirdGift>();
            var shop = new List<DodoBirdGift>();
            DestroyAllChildren(_calendarPanel.CalendarShop._shopContent);
            DestroyAllChildren(_calendarPanel.CalendarShop._boughtContent);

            foreach (var soGift in _config._gifts)
            {
                if (DodoBirdsService.Gifts.Any(g => g.id == soGift.Id))
                {
                    bought.Add(soGift);
                }
                else
                {
                    shop.Add(soGift);
                }
            }

            LoadBoughtButton(bought);
            LoadShopButtons(shop);
        }

        private void DestroyAllChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                Object.Destroy(child.gameObject);
            }
        }

        private void LoadShopButtons(List<DodoBirdGift> shopGifts)
        {
            var day = DateTime.Now.Day;
            var allIds = _config._gifts.Select(g => g.Id).ToList();
            var todayIds = new List<int>();

            switch (day % 2)
            {
                //0 - skyeng
                //1 - peperoni fresh
                //2 - 500 ddc
                //3 - any pizza
                //4 - skill box 
                //5 - year - кончились
                //6 - yandex plus - кончились
                case 0:
                    todayIds = new List<int> { allIds[0], allIds[1], allIds[2] };
                    break;
                case 1:
                    todayIds = new List<int> { allIds[0], allIds[3], allIds[4] };
                    break;
            }

            for (var index = 0; index < shopGifts.Count; index++)
            {
                var shopGift = shopGifts[index];

                if (!todayIds.Contains(shopGift.Id)) continue;

                var button = Object.Instantiate(index % 2 == 0 ? _leftShopButtonPrefab : _rightShopButtonPrefab,
                    _calendarPanel.CalendarShop._shopContent);

                button.SetData(shopGift, false, OnShopButtonClick);
            }
        }

        private void LoadBoughtButton(List<DodoBirdGift> boughtGifts)
        {
            for (var index = 0; index < boughtGifts.Count; index++)
            {
                var shopGift = boughtGifts[index];
                var button = Object.Instantiate(index % 2 == 0 ? _leftShopButtonPrefab : _rightShopButtonPrefab,
                    _calendarPanel.CalendarShop._boughtContent);
                button.SetData(shopGift, true, OnBoughtButtonClick);
            }
        }

        private void OnShopButtonClick(CalendarShopButton button)
        {
            button.ShowLoading();
            var giftConfig = _config.GetGiftByID(button.GiftId);

            DodoBirdsService.BuyDodoBirdsGift(button.GiftId,
                _ =>
                {
                    button.ShowSuccess();

                    var s = DOTween.Sequence();
                    s.AppendInterval(0.75f);
                    s.AppendCallback(() =>
                    {
                        _tapPresentPanel.Initialize(
                            giftConfig,
                            () =>
                            {
                                OnBoughtButtonClick(button);
                                ReloadShop();
                            }
                        );

                        _tapPresentPanel.Show();
                    });

                    ReloadShop();
                },
                button.ShowError, button.idempotencyKey);
        }


        private void OnBoughtButtonClick(CalendarShopButton button)
        {
            _presentInformationPanel.SetGiftData(_config.GetGiftByID(button.GiftId));
            _presentInformationPanel.Show();
        }

        private void SceneLoad()
        {
            ReloadShop();
            ReloadBalance();

            if (_calendarPresentStorage.OpenedPresentsData == null)
            {
                return;
            }

            CreatePresentButtons();
        }

        private void CreatePresentButtons()
        {
            for (var i = 0; i < _calendarPresentStorage.OpenedPresentsData.Count; i++)
            {
                var valuePair = _calendarPresentStorage.OpenedPresentsData[i];
                if (!_cutSceneConfig.TryGetCutScenePresentBox(valuePair.Date, out var presentBox))
                {
                    continue;
                }

                var presentButton = _uiFactory.CreatePresentButton(i % 2 == 0);
                presentButton.SetData(presentBox, _timeObserver.CurrentDay);
                _calendarPanel.AddNewPresentButton(presentButton);
                presentButton.ReceivePresent += OnReceivePresent;
                presentButton.ShowInfo += OnShowInfo;

                if (valuePair.IsOpen)
                {
                    presentButton.SetOpenedMode();
                }
            }
        }

        private void TakeNewPresentEvent(PresentBoxData obj)
        {
            var presentButton = _uiFactory.CreatePresentButton(_calendarPanel.Buttons.Count % 2 == 0);
            presentButton.SetData(obj, _timeObserver.CurrentDay);
            presentButton.ReceivePresent += OnReceivePresent;
            presentButton.ShowInfo += OnShowInfo;
            _calendarPanel.AddNewPresentButton(presentButton);
        }

        private void OnShowInfo(CalendarPresentButton calendarButton)
        {
            if (!calendarButton.IsOpen) return;

            ShowPresentInfo?.Invoke(calendarButton);
        }

        private void OnSwitchCalendar()
        {
            _calendarPanel.Switch();
        }

        private void OnReceivePresent(CalendarPresentButton calendarButton)
        {
            Debugging.Log(
                $"CalendarPanelController: OnPressPresentButton ->  is open {calendarButton.IsOpen} is can open {calendarButton.Data.IsCanOpen(_timeObserver.CurrentDay)}");

            if (!calendarButton.Data.IsCanOpen(_timeObserver.CurrentDay)) return;

            _currentTapPresent = calendarButton;

            _tapPresentPanel.Initialize(calendarButton, OnPresentTapDone);
            _tapPresentPanel.Show();
        }

        private void OnPresentTapDone()
        {
            if (_currentTapPresent == null) return;
            ShowPresentInfo?.Invoke(_currentTapPresent);
        }
    }
}