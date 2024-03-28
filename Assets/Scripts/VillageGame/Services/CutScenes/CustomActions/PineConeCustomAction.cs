using System;
using System.Collections;
using Data.Scripts.Audio;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Logic.Characters;
using VillageGame.Logic.Tree;
using VillageGame.Services.Storages;
using VillageGame.UI;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class PineConeCustomAction : CustomCutsceneAction
    {
        private readonly CharactersStorage _characterStorage;
        private readonly ChristmasTree _christmasTree;
        private readonly CoroutineRunner _coroutineRunner;

        private Character _guide;

        public PineConeCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _characterStorage = objectResolver.Resolve<CharactersStorage>();
            _christmasTree = objectResolver.Resolve<ChristmasTree>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.PineCone;
        }

        public override void Execute(Action action = null)
        {
            _guide = _characterStorage.GetCharacter(CharacterType.Guide);
            _christmasTree.AnimationEvents.EndPineConeAnimationEvent += OnEndTreeAnimation;
            _guide.AnimationEvents.EndPineConeAnimationEvent += OnEndCharacterAnimation;
        
            _guide.Animation.PlayPineCone();
            _coroutineRunner.StartCoroutine(PlayChristmasTreeAnimation());
        }

        private void OnEndCharacterAnimation()
        {
            _guide.Animation.EndCutScene();
            _guide.AnimationEvents.EndPineConeAnimationEvent -= OnEndCharacterAnimation;
        }

        private IEnumerator PlayChristmasTreeAnimation()
        {
            yield return new WaitForSeconds(0.75f);
            _christmasTree.Animation.SetProgressionState(1);
        }

        private void OnEndTreeAnimation()
        {
            _christmasTree.AnimationEvents.EndPineConeAnimationEvent -= OnEndTreeAnimation;
            EndCustomActionEvent?.Invoke(this);
        }
    }
}