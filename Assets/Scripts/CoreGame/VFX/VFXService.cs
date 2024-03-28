using System;
using System.Collections.Generic;
using System.Linq;
using SO;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VillageGame.Data;
using Object = UnityEngine.Object;

namespace CoreGame
{
    [Serializable]
    public class VFXService
    {
        private readonly Transform _particlesRoot;
        private readonly Dictionary<ParticleType, List<ParticleSystem>> _particlesPool = new();
        private readonly VFXConfig _vfxConfig;
        

        [Inject]
        public VFXService(Transform particleRoot, VFXConfig vfxConfig)
        {
            _particlesRoot = particleRoot;
            _vfxConfig = vfxConfig;
        }

        

        public ParticleSystem GetStopedParticle(ParticleType type, out ParticlePrefabData data)
        {
            data = _vfxConfig.GetData(type);

            if (!_particlesPool.ContainsKey(type))
            {
                _particlesPool.Add(type, new List<ParticleSystem>());
            }

            var particleTuple = _particlesPool[type];
            if (particleTuple == null)
            {
                particleTuple = new List<ParticleSystem>();
            }
            else
            {
                var stopedParticle = particleTuple.FirstOrDefault(p => p.isStopped);
                if (stopedParticle != null)
                {
                    return stopedParticle;
                }
            }

            if (data != null)
            {
                var newParticle = Object.Instantiate(data.Prefab, _particlesRoot);
                particleTuple.Add(newParticle);
                return newParticle;
            }

            return null;
        }

        public void Initialize()
        {
            Debug.Log("VFX Service Initialized!");
        }
    }
}