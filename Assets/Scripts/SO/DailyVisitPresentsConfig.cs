using SO.Data.Presents;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "DailyVisitPresentsConfig", menuName = "SO/DailyVisitPresentsConfig")]
    public class DailyVisitPresentsConfig : ScriptableObject
    {
        public PresentValueData[] DailyVisitPresents;
    }
}