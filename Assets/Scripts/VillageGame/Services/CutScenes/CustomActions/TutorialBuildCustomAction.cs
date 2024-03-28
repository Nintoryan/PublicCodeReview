using System;
using System.Collections;
using Tutorial;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.Logic.Tutorial;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;
using VillageGame.UI.Services;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class TutorialBuildCustomAction : CustomCutsceneAction
    {
        private const int TUTORIAL_INDEX = 3;
        private readonly TutorialStateStorage _tutorialStateStorage;
        private readonly MainButtonsController _mainButtonsController;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly RatingStorage _ratingStorage;
        
        private readonly BuildShopUIPanel _shopPanel;
        private readonly MainUIButton _buildShopMainButton;
        private readonly BuildingOnMapStorage _buildingOnMapStorage;
        
        private readonly TutorialFinger _tutorialFinger;
        private readonly TutorialCanvas _tutorialCanvas;
        private readonly TutorialBlackScreen _blackScreen;
        private readonly CurrencyStorage _currencyStorage;
        private readonly BuildTutorialMan _tutorialMan;

        private readonly BuildModeUIPanel _buildModeUIPanel;

        private readonly Transform _originalShopButtonTransform;
        private readonly Vector3 _constructionSitePosition = new(-13.7f, -3.75f, 0);

        public TutorialBuildCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _tutorialStateStorage = objectResolver.Resolve<TutorialStateStorage>();
            _mainButtonsController = objectResolver.Resolve<MainButtonsController>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            
            var uiFacade = objectResolver.Resolve<UIFacade>();
            _buildShopMainButton = uiFacade.FindPanel<MainUIPanel>().BuildingShopButton;
            _shopPanel = uiFacade.FindPanel<BuildShopUIPanel>();
            _buildModeUIPanel = uiFacade.FindPanel<BuildModeUIPanel>();
            
          
            _tutorialFinger = uiFacade.TutorialFinger;
            _tutorialCanvas = uiFacade.TutorialCanvas;
            _tutorialMan = uiFacade.TutorialManFacade.FindPanel<BuildTutorialMan>();
            _blackScreen = uiFacade.TutorialBlackScreen;
            
            _originalShopButtonTransform = _buildShopMainButton.transform.parent;
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.BuildTutorial;
        }

        public override void Execute(Action action = null)
        {
            if (_buildingOnMapStorage.TryGetBuilding(BuildingType.House, 0, out var post))
            {
                EndAction();
                return;
            }

            if (_currencyStorage.Current == 0)
            {
                _currencyStorage.Add(1);
            }
            
            _buildModeUIPanel.ExitModeButton.gameObject.SetActive(false);
            _tutorialStateStorage.InvokeStartTutorialWatching(TUTORIAL_INDEX);
            _buildShopMainButton.transform.SetParent(_tutorialCanvas.Body.transform);
            _tutorialFinger.transform.SetAsLastSibling();
            _mainButtonsController.CaptureControl(_buildShopMainButton,true);
            _blackScreen.SetActive(true);
            _tutorialMan.Show();

            _tutorialFinger.PlayBounceInWordPoint(
                _buildShopMainButton.body.position,
                new Vector3(0.4f, 0.5f),
                TutorialFinger.AngleType.DownLeft);

            _buildShopMainButton.ClickEvent += OnClickBuildShopMainButton;
            _shopPanel.ClickBuildingButtonEvent += OnBuildingButtonClicked;
            _shopPanel.PanelSwitchEvent += OnShopPanelSwitchEvent;
            _shopPanel.ModalPanel.BuildButtonClickedEvent += OnBuildButtonClicked;
            _ratingStorage.ChangeLevel += OnLevelUp;
        }


        private void OnClickBuildShopMainButton()
        {
            _buildShopMainButton.ClickEvent -= OnClickBuildShopMainButton;
            
            _tutorialMan.Hide();
            _mainButtonsController.FreeControl(_buildShopMainButton);
            _blackScreen.SetActive(false);
            _buildShopMainButton.transform.SetParent(_originalShopButtonTransform);

            _tutorialFinger.StopAnimation(withAnimation: false);
            _shopPanel.DisableTabsAndCloseButton();
        }

        private void OnShopPanelSwitchEvent(UIPanel _, bool isOpen)
        {
            if (isOpen)
            {
                _shopPanel.PanelSwitchEvent -= OnShopPanelSwitchEvent;
                _coroutineRunner.StartCoroutine(ShowTutorialFingerAtBuildingButton());
            }
        }

        private void OnLevelUp(int _)
        {
            _ratingStorage.ChangeLevel -= OnLevelUp;
            _tutorialFinger.StopAnimation(withAnimation:false);
            _shopPanel.EnableTabsAndCloseButton();
            _buildModeUIPanel.ExitModeButton.gameObject.SetActive(true);
            _shopPanel.ModalPanel.EnableBackButton();
            EndAction();
        }

        private void EndAction()
        {
            _tutorialStateStorage.InvokeEndTutorialWatching(TUTORIAL_INDEX);
            EndCustomActionEvent?.Invoke(this);
        }

        private IEnumerator ShowTutorialFingerAtBuildingButton()
        {
            _tutorialFinger.StopAnimation(withAnimation: false);
            yield return new WaitForSeconds(0.5f);
            var buildingButton = _shopPanel.FirstBuildingButton;

            _tutorialFinger.PlayBounceInWordPoint(buildingButton.body.position, offset: new Vector3(0.5f, -0.7f, 0));
        }

        private void OnBuildingButtonClicked()
        {
            _shopPanel.ClickBuildingButtonEvent -= OnBuildingButtonClicked;
            _coroutineRunner.StartCoroutine(ShowTutorialFingerAtModalBuildButton());
            _shopPanel.ModalPanel.DisableBackButton();
        }

        private void OnBuildButtonClicked(ShopBuildingUIButton.Data arg0)
        {
            _shopPanel.ModalPanel.BuildButtonClickedEvent -= OnBuildButtonClicked;
            _coroutineRunner.StartCoroutine(ShowTutorialFingerAtConstructionSite());
        }


        private IEnumerator ShowTutorialFingerAtModalBuildButton()
        {
            _tutorialFinger.StopAnimation(withAnimation: false);
            yield return new WaitForSeconds(0.5f);
            var buildButton = _shopPanel.ModalPanel.BuildButton;
            _tutorialFinger.PlayBounceInWordPoint(buildButton.transform.position, offset: new Vector3(0.5f, -0.2f, 0));
        }
        private IEnumerator ShowTutorialFingerAtConstructionSite()
        {
            _tutorialFinger.StopAnimation(withAnimation: false);
            yield return new WaitForSeconds(0.5f);
            _tutorialFinger.PlayBounceInWordPoint(_constructionSitePosition, offset: Vector3.zero);
        }
    }
}