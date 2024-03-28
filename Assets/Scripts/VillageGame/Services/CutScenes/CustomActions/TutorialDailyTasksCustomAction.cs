using System;
using Tutorial;
using UnityEngine;
using VContainer;
using VillageGame.Logic.Tutorial;
using VillageGame.UI;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;
using VillageGame.UI.Services;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class TutorialDailyTasksCustomAction : CustomCutsceneAction
    {
        private const int TUTORIAL_INDEX = 4;

        private readonly DailyTasksTutorialMan _tutorialMan;
        private readonly TutorialBlackScreen _blackScreen;
        private readonly DailyTasksPanel _dailyTasksPanel;
        private readonly TutorialFinger _tutorialFinger;
        private readonly TutorialCanvas _tutorialCanvas;
        private readonly MainButtonsController _mainIUButtonController;

        private readonly TutorialStateStorage _tutorialStateStorage;
        private readonly InputService _inputService;
        private readonly MainUIButton _dailyTasksMainButton;

        private readonly Transform _buttonParent,_panelParent;

        public TutorialDailyTasksCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            var uiFacade = objectResolver.Resolve<UIFacade>();
            _tutorialMan = uiFacade.TutorialManFacade.FindPanel<DailyTasksTutorialMan>();
            _tutorialCanvas = uiFacade.TutorialCanvas;
            _blackScreen = uiFacade.TutorialBlackScreen;
            _tutorialFinger = uiFacade.TutorialFinger;
            _dailyTasksPanel = uiFacade.FindPanel<DailyTasksPanel>();
            _dailyTasksMainButton = uiFacade.FindPanel<MainUIPanel>().DailyTasksButton;

            _inputService = objectResolver.Resolve<InputService>();
            _tutorialStateStorage = objectResolver.Resolve<TutorialStateStorage>();
            _mainIUButtonController = objectResolver.Resolve<MainButtonsController>();

            _buttonParent = _dailyTasksMainButton.transform.parent;
            _panelParent = _dailyTasksPanel.transform.parent;
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.DailyTasksTutorial;
        }

        public override void Execute(Action action = null)
        {
                _tutorialStateStorage.InvokeStartTutorialWatching(TUTORIAL_INDEX);
                _tutorialStateStorage.InvokeEndTutorialWatching(TUTORIAL_INDEX);

                _mainIUButtonController.CaptureControl(_dailyTasksMainButton, isShow: true);
                _dailyTasksPanel.PanelSwitchEvent += DailyTasksPanelOnPanelSwitchEvent;
                
                _blackScreen.SetActive(true);
                _tutorialMan.Show();
                _dailyTasksPanel.transform.SetParent(_tutorialCanvas.Body);
                _dailyTasksMainButton.transform.SetParent(_tutorialCanvas.Body);
                _tutorialFinger.transform.SetAsLastSibling();
                
                _tutorialFinger.PlayBounceInWordPoint(_dailyTasksMainButton.body.position,
                    angleType: TutorialFinger.AngleType.UpRight,
                    offset: new Vector3(-0.4f, -0.5f));
                
        }



        private void DailyTasksPanelOnPanelSwitchEvent(UIPanel arg1, bool isOpen)
        {
            if (isOpen)
            {
                _tutorialMan.Hide();
                _tutorialFinger.StopAnimation(withAnimation: false);
                _dailyTasksMainButton.transform.SetParent(_buttonParent);
            }
            if (!isOpen)
            {
                _dailyTasksPanel.PanelSwitchEvent -= DailyTasksPanelOnPanelSwitchEvent;
                _blackScreen.SetActive(false);
                _dailyTasksPanel.transform.SetParent(_panelParent);
                EndCustomActionEvent?.Invoke(this);
             
                _mainIUButtonController.FreeControl(_dailyTasksMainButton);
            }
       
        }
    }
}