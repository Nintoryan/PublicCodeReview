using System;
using CoreGame;
using VContainer;
using VillageGame.Logic.Curtains;
using VillageGame.Services.Snowdrifts;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class SnowFlakeFactory_Curtains_1 : CustomCutsceneAction
    {
        private readonly VFXController _vfxController;
        private readonly BigSnowdrifts _bigSnowDrift;
        private readonly CutSceneAnimator _cutSceneAnimator;
        
        public SnowFlakeFactory_Curtains_1(IObjectResolver objectResolver) : base(objectResolver)
        {
            _vfxController = objectResolver.Resolve<VFXController>();
            _bigSnowDrift = objectResolver.Resolve<BigSnowdrifts>();
            _cutSceneAnimator = objectResolver.Resolve<CutSceneAnimator>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.SnowFlakeFactory_Curtains_1;
        }

        public override void Execute(Action action = null)  
        {
            CloudCurtain.OnShownEvent += CloudCurtainOnShown;
            EndCustomActionEvent?.Invoke(this);
        }

        private void CloudCurtainOnShown()
        {
            CloudCurtain.OnShownEvent -= CloudCurtainOnShown;
            _bigSnowDrift.SetSnowflakeFactoryCutsceneState(true);
            _vfxController.StopAllSnowfall();
            _cutSceneAnimator.DisableTrain();
        }
    }
}   