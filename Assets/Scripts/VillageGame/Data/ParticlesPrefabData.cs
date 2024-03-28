using System;
using CoreGame;
using UnityEngine;

namespace VillageGame.Data
{

    [Serializable]
    public class ParticlePrefabData
    {
        public ParticleType Type;
        public ParticleSystem Prefab;
        public Vector3 Offset;
    }
}