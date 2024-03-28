using System;
using System.Collections;
using Data.Scripts.Audio;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Logic.Tree;
using VillageGame.Services.Storages;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class GrowChristmasTreeCustomAction : CustomCutsceneAction
    {
        private readonly ChristmasTree _christmasTree;
        private readonly CoroutineRunner _coroutineRunner;

        public GrowChristmasTreeCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _christmasTree = objectResolver.Resolve<ChristmasTree>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.GrownChristmasTree;
        }

        public override void Execute(Action action = null)
        {
            _christmasTree.AnimationEvents.EndGrowAnimationEvent += OnEndGrowAnimationEvent;
            _christmasTree.Animation.SetProgressionState(2);
            _coroutineRunner.StartCoroutine(PlayAudio());
        }

        private IEnumerator PlayAudio()
        {
            yield return new WaitForSeconds(0.75f);
            AudioManager.Instance.PlayAudioEvent(AudioEventType.TreeGrow);
        }
        private void OnEndGrowAnimationEvent()
        {
            EndCustomActionEvent?.Invoke(this);
        }
    }
}