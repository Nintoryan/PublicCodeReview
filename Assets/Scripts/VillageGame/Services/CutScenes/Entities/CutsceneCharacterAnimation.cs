using SO.Data.Characters;
using Spine.Unity;
using UnityEngine;

namespace VillageGame.Services.CutScenes.Entities
{
    public class CutsceneCharacterAnimation : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation _skeletonAnimation;

        enum State
        {
            None,
            BackIdle,
            BackMove,
            Chatting,
            Chatting2,
            Dancing,
            FrontIdle,
            FrontMove,
            Gift,
            Hello,
            PineCone
        }

        public void SetPostmanCharacter()
        {
            _skeletonAnimation.skeleton.SetSkin(CharacterType.Postman.ToString());
        }

        public void PlayFrontIdle()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, State.FrontIdle.ToString(), loop: true);
        }

        public void PlayBackIdle()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, State.BackIdle.ToString(), loop: true);
        }

        public void PlayFrontMove()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, State.FrontMove.ToString(), loop: true);
        }

        public void PlayBackMove()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, State.BackMove.ToString(), loop: true);
        }

        public void PlayHello()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, State.Hello.ToString(), loop: true);
            
        }

        
        public void PlayChatting()
        {
            var random = Random.Range(0, 2);
            if (random == 0)
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, State.Chatting.ToString(), loop: true);
            }
            else
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, State.Chatting2.ToString(), loop: true);
            }
        }
    }
}