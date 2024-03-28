using SO.Data;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "ProgressionConfig", menuName = "SO/ProgressionConfig")]
    public class ProgressionConfig : ScriptableObject
    {
        public PlayerLevelData[] playerLevels;
    }
}
