using System.Linq;
using CoreGame;
using UnityEngine;
using VillageGame.Data;

namespace SO
{
    [CreateAssetMenu(fileName = "VFXConfig", menuName = "SO/VFXConfig")]
    public class VFXConfig: ScriptableObject
    {
        public ParticlePrefabData[] ParticlesData;
        public ParticlePrefabData GetData(ParticleType type)
        {
            return ParticlesData.FirstOrDefault(p => p.Type == type);
        }
    }
    
}