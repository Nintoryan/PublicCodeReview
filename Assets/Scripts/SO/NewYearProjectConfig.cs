using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "NewYearProjectsConfig", menuName = "SO/NewYearProjectsConfig")]
    public class NewYearProjectConfig : ScriptableObject
    {
        public string Name;
        public int ID;
        public Sprite Icon;
        public int Cost;
        public int ProjectReward;
        public string Description = "Заполните описание";
    }
}
