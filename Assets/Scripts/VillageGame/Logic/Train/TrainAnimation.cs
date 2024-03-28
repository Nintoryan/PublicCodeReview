using Spine.Unity;
using UnityEngine;

namespace DefaultNamespace.VillageGame.Logic
{
    public class TrainAnimation : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation _animation;

        private enum AnimationState
        {
            Idle,
            Move
        }
        
        public void SetIdleAnimation()
        {
            _animation.AnimationState.SetAnimation(0, AnimationState.Idle.ToString() , true);
        }

        public void SetMoveAnimation()
        {
            _animation.AnimationState.SetAnimation(0, AnimationState.Move.ToString() , true);
        }
    }
}