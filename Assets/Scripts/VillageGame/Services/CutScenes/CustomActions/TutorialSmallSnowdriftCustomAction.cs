using System;
using SO.Data.Characters;
using Tutorial;
using UnityEngine;
using VContainer;
using VillageGame.Infrastructure.Factories;
using VillageGame.Logic.Snowdrifts;
using VillageGame.Logic.Curtains;
using VillageGame.Logic.Tutorial;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Panels;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class TutorialSmallSnowdriftCustomAction : CustomCutsceneAction
    {
        private const int TUTORIAL_ID = 2;

        private readonly CloudCurtain _cloudCurtain;
        private readonly CharactersStorage _characterStorage;
        private readonly CharactersFactory _characterFactory;
        private readonly TutorialFinger _tutorialFinger;
        private readonly ConstructionSiteService _constructionSiteService;
        
        private readonly Vector3 _tutorialFingerPos = new(-13.7f, -3.75f, 0);

        private readonly TutorialStateStorage _tutorialStateStorage;
        private readonly SnowdriftPanel _snowdriftPanel;
        private readonly TutorialBlackScreen _blackScreen;
        private readonly TutorialMan _tutorialMan;

        private bool _isWatchFedorTutor;

        public TutorialSmallSnowdriftCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _characterStorage = objectResolver.Resolve<CharactersStorage>();
            _characterFactory = objectResolver.Resolve<CharactersFactory>();
            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();
            _tutorialStateStorage = objectResolver.Resolve<TutorialStateStorage>();
            
            var uiFacade = objectResolver.Resolve<UIFacade>();
            _snowdriftPanel = uiFacade.FindPanel<SnowdriftPanel>();
            _tutorialFinger = uiFacade.TutorialFinger;
            _blackScreen = uiFacade.TutorialBlackScreen;
            _tutorialMan = uiFacade.TutorialManFacade.FindPanel<SnowdriftTutorialMan>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.SmallSnowdriftTutorial;
        }

        public override void Execute(Action action = null)
        {
            _tutorialStateStorage.InvokeStartTutorialWatching(TUTORIAL_ID);
            
            CreatePostMan();

            if (_constructionSiteService.TryGetConstructionSite(new Vector2Int(-11, 1), out var constructionSite)
                && constructionSite.Data.State != ConstructionSite.StateType.Snowdrift)
            {
                EndCustomActionEvent?.Invoke(this);
                return;
            }

            _snowdriftPanel.DisableCloseButton();
            
            _constructionSiteService.ClearSnowdriftEvent += ClearSnowdriftEvent;
            _snowdriftPanel.PanelSwitchEvent += SnowdriftPanelOnPanelSwitchEvent;
            _blackScreen.ClickEvent += OnClickBlackScreen;
                
            _blackScreen.SetActive(true);
            _tutorialMan.Show();
        }

        private void OnClickBlackScreen()
        {
            _blackScreen.ClickEvent -= OnClickBlackScreen;
            _blackScreen.SetActive(false);
            _tutorialMan.Hide();
            _tutorialFinger.PlayBounceInWordPoint(_tutorialFingerPos, offset: Vector3.zero);
        }

        private void CreatePostMan()
        {
            var postman = _characterFactory.CreateCharacter(CharacterType.Postman, 0);
            _characterStorage.Add(postman);
        }

        private void SnowdriftPanelOnPanelSwitchEvent(UIPanel arg1, bool arg2)
        {
            _tutorialFinger.StopAnimation(withAnimation: false);
            _snowdriftPanel.PanelSwitchEvent -= SnowdriftPanelOnPanelSwitchEvent;
        }

        private void ClearSnowdriftEvent(ConstructionSite obj)
        {
            _tutorialStateStorage.InvokeEndTutorialWatching(TUTORIAL_ID);

            _constructionSiteService.ClearSnowdriftEvent -= ClearSnowdriftEvent;
            _snowdriftPanel.EnableCloseButton();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}