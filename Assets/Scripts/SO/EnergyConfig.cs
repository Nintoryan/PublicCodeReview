using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "EnergyConfig", menuName = "SO/EnergyConfig")]
    public class EnergyConfig : ScriptableObject
    {
        public int MaxEnergy = 8;
        public int EnergyRecoveryTime = 1800;
    }
}
