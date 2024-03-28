using System;
using Data.Scripts.Audio;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class OpenSuitcaseCustomAnimation : CustomCutsceneAction
    {
        private readonly CutSceneAnimator _cutsceneAnimator;

        public OpenSuitcaseCustomAnimation(IObjectResolver objectResolver) : base(objectResolver)
        {
            _cutsceneAnimator = objectResolver.Resolve<CutSceneAnimator>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.OpenSuitcase;
        }

        public override void Execute(Action action = null)
        {
            _cutsceneAnimator.Suitcase.SetOpenSprite();
            _cutsceneAnimator.Suitcase.PlayOpenParticle();
            AudioManager.Instance.PlayAudioEvent(AudioEventType.OpenSuitcase);
            EndCustomActionEvent?.Invoke(this);
        }
    }
}