using System;
using CoreGame;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class SnowFlakeFactory_BigSnow : CustomCutsceneAction
    {
        private readonly VFXController _vfxController;
        public SnowFlakeFactory_BigSnow(IObjectResolver objectResolver) : base(objectResolver)
        {
            _vfxController = objectResolver.Resolve<VFXController>();
        }
        
        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.SnowFlakeFactory_BigSnow;
        }

        public override void Execute(Action action = null)
        {
            _vfxController.PlayBigSnowfall();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}