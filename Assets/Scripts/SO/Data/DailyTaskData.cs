using UnityEngine;
using VillageGame.Data;

namespace SO.Data
{
    [CreateAssetMenu(fileName = "DailyTaskData",  menuName = "SO/Data/DailyTaskData")]
    public class DailyTaskData : ScriptableObject
    {
        public ConditionData Condition;
        
        [Header("Будет отображаться на панельке задания")]
        [TextArea(3,10)]public string Desctiption;
        public Sprite Icon;
        
        [Header("Награды")]
        public int RatingReward;
        public int CurrencyReward;
        public int HintReward;
        public int ShuffleReward;
        public int DecorationRewardID = -1;
    }
}