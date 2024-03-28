using System;
using System.Collections.Generic;
using System.Linq;
using Util;
using VContainer;
using VillageGame.Services.CutScenes.CustomActions;

namespace VillageGame.Services.CutScenes
{
    public class CutSceneActionsExecutor
    {
        private List<CustomCutsceneAction> _actions;
        public static Action EndEvent;

        public static bool IsWatching { get; private set; }

        public CustomCutsceneActionType IsWatchingType;
        
        [Inject]
        public CutSceneActionsExecutor(IObjectResolver objectResolver)
        {
            InitializeActions(objectResolver);
        }

        private void InitializeActions(IObjectResolver objectResolver)
        {
            _actions = new List<CustomCutsceneAction>
            {
                new AfterTenMinutesCustomAction(objectResolver),
                new ArrivalOfGuideCustomAction(objectResolver),
                new ClearBigSnowdriftCustomAction(objectResolver),
                new GrowChristmasTreeCustomAction(objectResolver),
                new HelloPostmanCustomAction(objectResolver),
                new OpenSuitcaseCustomAnimation(objectResolver),
                new PineConeCustomAction(objectResolver),
                new ShowEnterNameWindowCustomAction(objectResolver),
                new SnowFlakeFactory_BigSnow(objectResolver),
                new SnowFlakeFactory_Curtains_1(objectResolver),
                new SnowFlakeFactory_Curtains_2(objectResolver),
                new SnowFlakeFactory_SmallSnow(objectResolver),
                new TutorialBuildCustomAction(objectResolver),
                new TutorialCalendarCustomAction(objectResolver),
                new TutorialCoreGameCustomAction(objectResolver),
                new TutorialDailyTasksCustomAction(objectResolver),
                new TutorialSmallSnowdriftCustomAction(objectResolver),
                new SculptureParkDestroyedCustomAction(objectResolver),
                new SculptureParkRestoredCustomAction(objectResolver),
                new BigfootAppearCustomAction(objectResolver),
                new BigfootDisappearCustomAction(objectResolver),
                new EndStoryCustomAction(objectResolver)
            };

            foreach (var action in _actions)
            {
                action.EndCustomActionEvent += OnActionEnd;
            }
        }

        public void ExecuteAction(CustomCutsceneActionType actionType)
        {
            if (actionType == CustomCutsceneActionType.None)
            {
                Debugging.Log($"CutSceneActionsExecutor: ExecuteAction -> 1.Execute none and push end event", ColorType.Lightblue);
                EndEvent?.Invoke();
            }
            else
            {
                var action = _actions.FirstOrDefault(a => a.GetActionType() == actionType);
                if (action == null)
                {
                    EndEvent?.Invoke();
                    Debugging.Log($"CutSceneActionsExecutor: ExecuteAction -> 2.Execute action == null push end event", ColorType.Lightblue);
                }
                else
                {
                    IsWatching = true;
                    IsWatchingType = actionType;
                    action.Execute();
                    Debugging.Log($"CutSceneActionsExecutor: ExecuteAction -> 3.Execute action {actionType}", ColorType.Lightblue);
                }
            }
        }

        private void OnActionEnd(CustomCutsceneAction action)
        {
            IsWatching = false;
            IsWatchingType = CustomCutsceneActionType.None;
            EndEvent?.Invoke();
            Debugging.Log($"CutSceneActionsExecutor: ExecuteAction ->  On Action end {action.GetActionType()}", ColorType.Lightblue);
        }
    }
}