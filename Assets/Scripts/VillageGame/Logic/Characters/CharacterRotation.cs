using Spine.Unity;
using UnityEngine;

namespace VillageGame.Logic.Characters
{
    public class CharacterRotation: MonoBehaviour
    {
        [SerializeField] private CharacterMove _characterMove;
        [SerializeField] private SkeletonMecanim _skeletonMecanim;

        private void Update()
        {
            if(Mathf.Abs(transform.position.x - _characterMove.LookTarget.x)<0.1f)
                return;
            
            var isRight = transform.position.x < _characterMove.LookTarget.x;
            if(_skeletonMecanim != null)
            _skeletonMecanim.skeleton.ScaleX = isRight ? 1 : -1;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position,0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_characterMove.LookTarget,0.2f);
        }
    }
}