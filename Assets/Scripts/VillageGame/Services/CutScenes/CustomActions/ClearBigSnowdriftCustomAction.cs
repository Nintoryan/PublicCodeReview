using System;
using Data.Scripts.Audio;
using VContainer;
using VillageGame.Services.Snowdrifts;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class ClearBigSnowdriftCustomAction: CustomCutsceneAction
    {
        private readonly BigSnowdrifts _bigSnowDrift;

        public ClearBigSnowdriftCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _bigSnowDrift = objectResolver.Resolve<BigSnowdrifts>();
        }



        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.ClearBigSnowdrift;
        }

        public override void Execute(Action action = null)
        {
            _bigSnowDrift.PlayHideAnimation();
            AudioManager.Instance.PlayAudioEvent(AudioEventType.ClearBigSnowdrift);
            
            EndCustomActionEvent?.Invoke(this);
        }
    }
}