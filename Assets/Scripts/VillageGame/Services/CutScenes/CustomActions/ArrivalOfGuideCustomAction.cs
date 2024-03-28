using System;
using System.Collections;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Logic.Cameras;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class ArrivalOfGuideCustomAction : CustomCutsceneAction
    {
        private readonly CutSceneAnimator _cutSceneAnimator;
        private readonly CutSceneCamera _cutSceneCamera;
        private readonly CoroutineRunner _coroutineRunner;

        public ArrivalOfGuideCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _cutSceneAnimator = objectResolver.Resolve<CutSceneAnimator>();
            _cutSceneCamera = objectResolver.Resolve<CutSceneCamera>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();
            _cutSceneAnimator.EndAnimationEvent += EndAnimation;
        }

        ~ArrivalOfGuideCustomAction()
        {
            _cutSceneAnimator.EndAnimationEvent -= EndAnimation;
        }
        private void EndAnimation()
        {
            EndCustomActionEvent?.Invoke(this);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.ArrivalOfGuidAnimation;
        }

        public override void Execute(Action action = null)
        {
            _coroutineRunner.StartCoroutine(StartAction());
        }

        public IEnumerator StartAction()
        {
            yield return new WaitUntil(() => !_cutSceneCamera.IsMovement);
            _cutSceneAnimator.PlayArrivalOfGuidAnimation();
        }
    }
}