using System;
using CoreGame;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class SnowFlakeFactory_SmallSnow : CustomCutsceneAction
    {
        protected readonly VFXController _vfxController;
        
        public SnowFlakeFactory_SmallSnow(IObjectResolver objectResolver) : base(objectResolver)
        {
            _vfxController = objectResolver.Resolve<VFXController>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.SnowFlakeFactory_SmallSnow;
        }

        public override void Execute(Action action = null)
        {
            _vfxController.PlaySmallSnowfall();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}