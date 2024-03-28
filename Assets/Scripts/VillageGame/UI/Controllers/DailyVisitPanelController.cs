using DG.Tweening;
using SO;
using SO.Data.Presents;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using VillageGame.Infrastructure.Factories;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using VillageGame.Services.Presents;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class DailyVisitPanelController : IInitializable
    {
        private readonly ApplicationObserver _applicationObserver;
        private readonly DailyVisitPanel _visitPanel;
        private readonly UIFactory _uiFactory;

        private readonly DailyVisitService _visitService;
        
        private readonly CutSceneStartController _cutSceneStartController;

        private readonly DailyVisitPresentsConfig _presentsConfig;
        private readonly MainUIPanel _mainUIPanel;

        private DailyVisitButton _currentReadyButton;

        [Inject]
        public DailyVisitPanelController(IObjectResolver objectResolver)
        {
            _mainUIPanel = objectResolver.Resolve<UIFacade>().FindPanel<MainUIPanel>();
            _applicationObserver = objectResolver.Resolve<ApplicationObserver>();
            _presentsConfig = objectResolver.Resolve<DailyVisitPresentsConfig>();
            _visitPanel = objectResolver.Resolve<UIFacade>().FindPanel<DailyVisitPanel>();
            _uiFactory = objectResolver.Resolve<UIFactory>();
            _visitService = objectResolver.Resolve<DailyVisitService>();
            
            _cutSceneStartController = objectResolver.Resolve<CutSceneStartController>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }
        
        ~DailyVisitPanelController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _applicationObserver.SceneLoadEvent += OnSceneLoadEvent;
            }
            else
            {
                _applicationObserver.SceneLoadEvent -= OnSceneLoadEvent;
            }
        }

        private void OnSceneLoadEvent()
        {
            if (_visitService.IsEmpty() 
                || _visitService.VisitData.is_open_today 
                || !_cutSceneStartController.TutorialSceneIsWatched())
                return;

            
            _visitPanel.PressButtonEvent += PressButtonEvent;
            
            for (var index = 0; index < _presentsConfig.DailyVisitPresents.Length; index++)
            {
                var visitPresent = _presentsConfig.DailyVisitPresents[index];

                var button = _uiFactory.CreateDailyVisitButton();
                var amountToShow = -1;
                if (visitPresent is CurrencyPresentValueData c)
                {
                    amountToShow = c.Count;
                }
                button.Init(dayNumber: index, rewardImage: visitPresent.Icon,amountToShow);

                if (index == _presentsConfig.DailyVisitPresents.Length - 1)
                {
                    var le = button.gameObject.AddComponent<LayoutElement>();
                    le.ignoreLayout = true;
                    var rt = button.GetComponent<RectTransform>();
                    rt.anchorMin = new Vector2(0.5f, 0.5f);
                    rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = new Vector2(0, -610);
                }
                
                if (index < _visitService.VisitData.next_present_number)
                {
                    button.SetReceived();
                }
                else if (index == _visitService.VisitData.next_present_number)
                {
                    _currentReadyButton = button;
                    button.SetReady();
                }
                else
                {
                    button.SetNotReady();
                }
            }
            
            _visitPanel.Show();
        }

        private void PressButtonEvent()
        {
            if (_currentReadyButton != null)
            {
                _currentReadyButton.SetReceived(0.5f);
            }
            var s = DOTween.Sequence();
            s.AppendInterval(0.75f);
            s.AppendCallback(() =>
            {
                _visitPanel.Hide();
                _visitService.OpenLastPreset();
            });
        }
    }
}