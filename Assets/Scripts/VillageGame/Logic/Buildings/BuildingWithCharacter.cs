using Spine.Unity;
using UnityEngine;

namespace VillageGame.Logic.Buildings
{
    public class BuildingWithCharacter : Building
    {
        [SerializeField] private GameObject _characterView;
        [SerializeField] private Animator _characterAnimator;
        [SerializeField] private SkeletonAnimation _skeletonAnimation;

        public GameObject CharacterView { get => _characterView; set => _characterView =  value ; }
        public Animator CharacterAnimator { get => _characterAnimator; set => _characterAnimator =  value ; }

        public void StopCharacter()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, "FrontIdle", true);
        }
    }
}
