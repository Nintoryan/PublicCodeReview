using SO.Data;
using SO.Data.Characters;
using Spine.Unity;
using UnityEngine;
using VillageGame.Logic.Tree;
using VillageGame.Services.Characters;

namespace VillageGame.Logic.Characters
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterMove _characterMove;
        [SerializeField] private SkeletonMecanim _skeletonMecanim;
        [SerializeField] private CharacterAnimation _characterAnimation;
        [SerializeField] private CharacterRotation _characterRotation;
        [SerializeField] private CharacterAnimationEvents _characterAnimationEvents;
        public CharacterRotation Rotation => _characterRotation;
        public CharacterMove Move => _characterMove;
        public CharacterAnimation Animation => _characterAnimation;
        public CharacterAnimationEvents AnimationEvents => _characterAnimationEvents;
        
        public CharacterData Data { get; private set; }
        public int BuildingId { get; private set; }

        public void Init(
            CharacterData data, 
            int buildingId, 
            CharactersNavigationService navigationService,
            ChristmasTree tree)
        {
            Data = data;
            BuildingId = buildingId;
            _characterMove.Init(navigationService, tree);
            
            if (data != null && _skeletonMecanim != null)
            {
                _skeletonMecanim.skeleton.SetSkin(data.Type.ToString());
            }
        }

        public void TeleportToPosition(Vector3 position, bool isUp, bool isLeft)
        {
            var self = transform;
            
            self.position = position;
            _characterMove.LookTarget = self.position + new Vector3(isLeft ? -1 : 1, isUp ? 1 : -1, 0);
        }
        public void TeleportToPosition(Vector3 position)
        {
            var self = transform;
            self.position = position;
        }
    }
}