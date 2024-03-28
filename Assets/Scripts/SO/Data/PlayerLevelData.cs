using UnityEngine;

namespace SO.Data
{
    [CreateAssetMenu(fileName = "PlayerLevelData", menuName = "SO/Data/PlayerLevelData")]
    public class PlayerLevelData : ScriptableObject
    {
        public int RatingRequirement;
        public int RatingCapacity;
        public int CurrencyCapacity;
        
        public BuildingData[] UnlockedBuildings;
        public NewYearProjectConfig[] NewYearProjects;
    }
}
