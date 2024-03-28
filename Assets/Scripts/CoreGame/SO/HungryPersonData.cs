using System.Collections.Generic;
using UnityEngine;
using CoreGame.Data;

namespace CoreGame.SO
{
    [CreateAssetMenu(fileName = "HungryPersonData", menuName = "SO/CoreGame/HungryPersonData")]
    public class HungryPersonData : ScriptableObject
    {
        public List<ObjectType> RequiredObjects;
    }
}
