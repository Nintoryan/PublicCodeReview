using System;
using System.Collections.Generic;
using UnityEngine;
using Util;
using VillageGame.Logic.Characters;

namespace VillageGame.Logic.Buildings
{
    [Serializable]
    public class BuildingLookPosition
    {
        [SerializeField] private List<Transform> _points;
        private readonly Dictionary<Character, Transform> _lookingPoints = new();

        public Vector3 GetCaptureLookPoint(Character owner)
        {
            if (_points.Count == 0)
            {
                Debugging.Log("Не хватило всем мест у домиков!!! Добавьте больше мест!");
                return Vector3.zero;
            }

            if (_lookingPoints.ContainsKey(owner))
            {
                return _lookingPoints[owner].position;
            }
       
            var point = _points[0];
            _points.RemoveAt(0);
            _lookingPoints.Add(owner,point);
                
            return point.position;
        }

        public Vector3 GetLastCaptureLookPoint(Character owner)
        {
            if (_points.Count == 0)
            {
                Debugging.Log("Не хватило всем мест у домиков!!! Добавьте больше мест!");
                return Vector3.zero;
            }

            if (_lookingPoints.ContainsKey(owner))
            {
                return _lookingPoints[owner].position;
            }
       
            var point = _points[^1];
            _points.Remove(point);
            _lookingPoints.Add(owner,point);
                
            return point.position;


        }
        public void FreePoint(Character owner)
        {
            if (_lookingPoints.ContainsKey(owner))
            {
                _points.Add(_lookingPoints[owner]);
                _lookingPoints.Remove(owner);
            }
        }

    }
}