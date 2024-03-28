using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Util;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.Logic.Tree;
using VillageGame.Services.Buildings;
using VillageGame.Services.Storages;
using Character = VillageGame.Logic.Characters.Character;

namespace VillageGame.Services.Characters
{
    public class CharactersNavigationService
    {
        private readonly BuildingOnMapStorage _buildingOnMapStorage;
        private readonly ChristmasTree _christmasTree;
        private readonly BuildingAreaService _buildingAreaService;

        private readonly Dictionary<Character, Transform> _treeLookingPoints = new();

        [Inject]
        public CharactersNavigationService(IObjectResolver objectResolver)
        {
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _christmasTree = objectResolver.Resolve<ChristmasTree>();
            _buildingAreaService = objectResolver.Resolve<BuildingAreaService>();
        }

        public Vector3 GetRandomPoint()
        {
            return _buildingAreaService.GetRandomAreaPosition();
        }

        public Vector3 GetHomePoint(int id)
        {
            return _buildingOnMapStorage.TryGetBuilding(BuildingType.House, id, out var building)
                ? building.transform.position
                : GetRandomPoint();
        }

        public Vector3 CaptureChristmasTreePoint(Character owner)
        {
            if (_christmasTree._lookPositions.Count == 0)
            {
                Debugging.Log("Не хватило всем мест у Ёлки!!! Добавьте больше мест!");
                return Vector3.zero;
            }

            if (_treeLookingPoints.ContainsKey(owner))
            {
                return _treeLookingPoints[owner].position;
            }
            _christmasTree._lookPositions.Sort((t1, t2) =>
            {
                var position = owner.transform.position;
                return (int)(Vector3.Distance(t1.position, position) - Vector3.Distance(t2.position, position));
            });
            
            var point = _christmasTree._lookPositions[0];
            _christmasTree._lookPositions.RemoveAt(0);
                _treeLookingPoints.Add(owner,point);
                
            return point.position;
            
            /*return NavMesh.SamplePosition(targetPosition, out var hit, 4, NavMesh.AllAreas)
                ? hit.position
                : _christmasTree.transform.position;*/
        }

        public void FreeTreePoint(Character owner)
        {
            if (_treeLookingPoints.ContainsKey(owner))
            {
                _christmasTree._lookPositions.Add(_treeLookingPoints[owner]);
                _treeLookingPoints.Remove(owner);
            }
        }

        public bool IsPositionOnNavMesh(Vector3 position)
        {
            return NavMesh.SamplePosition(position, out var hit, 0.1f, NavMesh.AllAreas);
        }
        
    }
}