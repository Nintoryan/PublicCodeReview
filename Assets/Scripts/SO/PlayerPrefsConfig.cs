using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "PlayerPrefsConfig", menuName = "SO/PlayerPrefsConfig")]
    public class PlayerPrefsConfig : ScriptableObject
    {
        public string ReloadFlag = "IsReload";
        public string VictoryFlag = "IsVictory";
        public string PlayingLevelFlag = "PlayingLevel";
    }
}
