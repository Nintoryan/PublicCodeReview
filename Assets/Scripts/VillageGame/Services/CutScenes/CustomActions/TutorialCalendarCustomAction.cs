using System;
using System.Collections;
using Tutorial;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Logic.Tutorial;
using VillageGame.UI;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;
using VillageGame.UI.Services;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class TutorialCalendarCustomAction : CustomCutsceneAction
    {
        private const int TUTORIAL_ID = 1;

        private readonly PresentInformationPanel _informationPresentPanel;
        private readonly TapPresentPanel _tapPresentPanel;
        private readonly TutorialStateStorage _tutorialStateStorage;
        private readonly TutorialBlackScreen _blackScreen;
        private readonly MainUIButton _calendarMainButtonButton;
        private readonly TutorialFinger _tutorialFinger;
        private readonly Transform _originalButtonParen;
        private readonly TutorialCanvas _tutorialCanvas;
        private readonly CalendarPanel _calendarPanel;
        private readonly MainButtonsController _mainButtonsController;
        private readonly CoroutineRunner _coroutineRunner;


        private CalendarPresentButton _firstPresentButton;
        private readonly MainUIPanel _mainPanel;
        private readonly CalendarTutorialMan _tutorialMan;

        public TutorialCalendarCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();
            _mainButtonsController = objectResolver.Resolve<MainButtonsController>();
            _tutorialStateStorage = objectResolver.Resolve<TutorialStateStorage>();

            var uiFacade = objectResolver.Resolve<UIFacade>();
            _informationPresentPanel = uiFacade.FindPanel<PresentInformationPanel>();
            _tapPresentPanel = uiFacade.FindPanel<TapPresentPanel>();
            _mainPanel = uiFacade.FindPanel<MainUIPanel>();
            _calendarPanel = uiFacade.FindPanel<CalendarPanel>();
            _tutorialCanvas = uiFacade.TutorialCanvas;
            _tutorialFinger = uiFacade.TutorialFinger;
            _tutorialMan = uiFacade.TutorialManFacade.FindPanel<CalendarTutorialMan>();
            _blackScreen = uiFacade.TutorialBlackScreen;

            _calendarMainButtonButton = _mainPanel.CalendarButton;
            _originalButtonParen = _calendarMainButtonButton.transform.parent;
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.CalendarTutorial;
        }

        public override void Execute(Action action = null)
        {
            _tutorialStateStorage.InvokeStartTutorialWatching(TUTORIAL_ID);
            _tutorialStateStorage.InvokeEndTutorialWatching(TUTORIAL_ID);

            _calendarMainButtonButton.ClickEvent += OnClickMainCalendarButton;
            _calendarPanel.PanelSwitchEvent += OnCalendarSwitch;
            _tapPresentPanel.PanelSwitchEvent += OnTapPresentPanelSwitch;
            
            _calendarMainButtonButton.transform.SetParent(_tutorialCanvas.Body);
            _calendarPanel.DisableTabsAndCloseButton();
            _tutorialFinger.transform.SetAsLastSibling();
            _blackScreen.SetActive(true);
            _tutorialMan.Show(OnShown: () =>
            {
                _mainButtonsController.CaptureControl(_calendarMainButtonButton, true);
                _tutorialFinger.PlayBounceInWordPoint(_calendarMainButtonButton.body.position, new Vector3(0.4f, -0.5f),
                    angleType: TutorialFinger.AngleType.UpLeft);
            });
      
        }
        
        private void OnCalendarSwitch(UIPanel arg1, bool isOpen)
        {
            if (isOpen)
            {
                _firstPresentButton = _calendarPanel.GetFirstPresentButton();
                _firstPresentButton.UnblockClickEvent += OnFirstPresentButtonClick;
                _coroutineRunner.StartCoroutine(ShowTutorialFingerAtPresentButton());
            }
            else
            {
                _calendarPanel.EnableTabsAndCloseButton();
                _calendarPanel.PanelSwitchEvent -= OnCalendarSwitch;
            }
        }

        private void OnPresentInformationSwitch(UIPanel panel, bool isOpen)
        {
            EndCustomActionEvent?.Invoke(this);
            _informationPresentPanel.PanelSwitchEvent -= OnPresentInformationSwitch;
        
        }

        private void OnTapPresentPanelSwitch(UIPanel arg1, bool isOpen)
        {
            if (isOpen)
            {
                _tutorialFinger.transform.SetParent(_tapPresentPanel.PresentButton.body);
                _tutorialFinger.transform.SetAsLastSibling();
                _tutorialFinger.PlayBounceInWordPoint(
                    _tapPresentPanel.PresentButton.body.position,
                    offset: new Vector3(0.7f, -0.7f, 0));
            }
            else
            {
                _tutorialFinger.SetDefaultParent(); 
                _tutorialFinger.transform.SetAsLastSibling();
                _tapPresentPanel.PanelSwitchEvent -= OnTapPresentPanelSwitch;
                _tutorialFinger.StopAnimation(withAnimation: false);
            }
        }

        private IEnumerator ShowTutorialFingerAtPresentButton()
        {
            _tutorialFinger.StopAnimation(withAnimation: false);
            yield return new WaitForSeconds(0.5f);
            _tutorialFinger.PlayBounceInWordPoint(_firstPresentButton.ButtonBody.position, offset: new Vector3(0.5f, -0.2f, 0));
        }

        private void OnClickMainCalendarButton()
        {
            _calendarMainButtonButton.UnblockClickEvent -= OnClickMainCalendarButton;
            
            _mainButtonsController.FreeControl(_calendarMainButtonButton);
            _blackScreen.SetActive(false);
            _tutorialMan.Hide();
            _calendarMainButtonButton.transform.SetParent(_originalButtonParen);
        }

        private void OnFirstPresentButtonClick(CalendarPresentButton presentButton)
        {
            _tutorialFinger.StopAnimation(withAnimation: false);
            _firstPresentButton.UnblockClickEvent -= OnFirstPresentButtonClick;
            _tapPresentPanel.Error += OnTapError;
            _tapPresentPanel.Success += OnTapSuccess;
            
        }

        private void OnTapSuccess()
        {
            _tapPresentPanel.Success -= OnTapSuccess;
            
            _informationPresentPanel.PanelSwitchEvent += OnPresentInformationSwitch;
        }

        private void OnTapError()
        {
            _tapPresentPanel.Error -= OnTapError;
            
            EndCustomActionEvent?.Invoke(this);
            _tutorialStateStorage.InvokeEndTutorialWatching(TUTORIAL_ID);
        }
    }
}