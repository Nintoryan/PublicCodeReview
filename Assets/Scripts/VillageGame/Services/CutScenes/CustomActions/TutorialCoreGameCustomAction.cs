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
    public class TutorialCoreGameCustomAction : CustomCutsceneAction
    {
        private const int TUTORIAL_ID = 0;
        
        private readonly TutorialStateStorage _tutorialStateStorage;
        private readonly CoreGameLoadService _coreSceneLoader;
        private readonly CutSceneAnimator _cutSceneAnimator;
        
        private readonly TutorialBlackScreen _blackScreen;
        private readonly MainButtonsController _mainButtonsController;
        private readonly MainUIButton _coreGameButton;
        private readonly TutorialFinger _tutorialFinger;
        private readonly TutorialCanvas _tutorialCanvas;
        
        private readonly Transform _originalButtonParen;

        public TutorialCoreGameCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _coreSceneLoader = objectResolver.Resolve<CoreGameLoadService>();
            var uiFacade = objectResolver.Resolve<UIFacade>();
            _coreGameButton = uiFacade.FindPanel<MainUIPanel>().CoreGameButton;
            _mainButtonsController = objectResolver.Resolve<MainButtonsController>();
            _tutorialCanvas = uiFacade.TutorialCanvas;
            _tutorialFinger = uiFacade.TutorialFinger;
            _blackScreen = uiFacade.TutorialBlackScreen;
            _originalButtonParen = _coreGameButton.transform.parent;

            _tutorialStateStorage = objectResolver.Resolve<TutorialStateStorage>();
            _cutSceneAnimator = objectResolver.Resolve<CutSceneAnimator>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.CoreGameTutorial;
        }

        public override void Execute(Action action = null)
        {
            _tutorialStateStorage.InvokeStartTutorialWatching(TUTORIAL_ID);

            _coreGameButton.BlockDefaultClickEvent(true);
            _coreGameButton.transform.SetParent(_tutorialCanvas.Body);
            _tutorialFinger.transform.SetAsLastSibling();
            _mainButtonsController.CaptureControl(_coreGameButton,true);

            _blackScreen.SetActive(true);
            _tutorialFinger.PlayBounceInWordPoint(_coreGameButton.body.position, new Vector3(-0.4f, 0.5f),
                TutorialFinger.AngleType.DownRight);

            _coreGameButton.UnblockClickEvent += ClickEvent;
        }

        private void ClickEvent()
        {
            _tutorialStateStorage.InvokeEndTutorialWatching(TUTORIAL_ID);
            
            _coreGameButton.UnblockClickEvent -= ClickEvent;
            
            _tutorialFinger.StopAnimation(onHidden: () =>
            {
                _coreGameButton.transform.SetParent(_originalButtonParen);
                _coreGameButton.BlockDefaultClickEvent(false);
                
                _mainButtonsController.FreeControl(_coreGameButton);

                _blackScreen.SetActive(false);
                _cutSceneAnimator.Suitcase.SetOpenSprite();

                EndCustomActionEvent?.Invoke(this);
                _coreSceneLoader.LoadCoreGameScene();
            });
        }
    }
}