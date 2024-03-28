using System.Collections.Generic;
using UnityEngine;

namespace CoreGame.SO
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "SO/CoreGame/LevelDatabase")]
    public class LevelDatabase : ScriptableObject
    {
        public int CyclicLevelStart;
        public List<RandomizedLevelData> Levels;
    }
}
