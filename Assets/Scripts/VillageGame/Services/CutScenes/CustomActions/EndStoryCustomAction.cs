using System;
using VContainer;
using VillageGame.UI;
using VillageGame.UI.Panels.Warnings;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class EndStoryCustomAction : CustomCutsceneAction
    {
        private readonly UIFacade _uiFacade;

        public EndStoryCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _uiFacade = objectResolver.Resolve<UIFacade>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.EndStoryCustomAction;
        }

        public override void Execute(Action action = null)
        {
            _uiFacade.FindWarningPanel<EndStoryWarningPanel>().Show();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}
