using System;
using Util;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class HelloPostmanCustomAction : CustomCutsceneAction
    {
        private readonly CutSceneAnimator _cutsceneAnimator;

        public HelloPostmanCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _cutsceneAnimator = objectResolver.Resolve<CutSceneAnimator>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.HelloPostman;
        }

        public override void Execute(Action action = null)
        {
            Debugging.Log("1",ColorType.Blue);
            _cutsceneAnimator.PlayHelloPostmanAnimation();
            _cutsceneAnimator.EndAnimationEvent += EndAnimation;
        }

        private void EndAnimation()
        {
            _cutsceneAnimator.EndAnimationEvent -= EndAnimation;
            EndCustomActionEvent?.Invoke(this);
        }
    }
}