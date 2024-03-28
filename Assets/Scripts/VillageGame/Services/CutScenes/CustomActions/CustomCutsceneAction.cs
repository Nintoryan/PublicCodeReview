using System;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public abstract class CustomCutsceneAction
    {
        public Action<CustomCutsceneAction> EndCustomActionEvent;

        protected CustomCutsceneAction(IObjectResolver objectResolver)
        {
        }

        public abstract CustomCutsceneActionType GetActionType();
        public abstract void Execute(Action action = null);
    }
}