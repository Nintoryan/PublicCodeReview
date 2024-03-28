using SO;
using System;
using UnityEngine;
using VContainer;

namespace CoreGame
{
    public class VFXAdapter: MonoBehaviour
    {
        [Header("Services")] 
        [SerializeField] private Transform _vfxRoot;
        [SerializeField] private GridController _gridController;
        private VFXService _vfxService;

        [Inject]
        private void Construct(VFXConfig vfxConfig)
        {
            _vfxService = new VFXService(_vfxRoot, vfxConfig);
        }
        
        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _gridController.MergeObjectEvent += OnMergeObjectEvent;
                _gridController.UnpackObjectEvent += OnUnpackObjectEvent;
                _gridController.BirdMergedEvent += OnBirdMergedEvent;
                _gridController.BirdSpawnedEvent += OnBirdSpawnedEvent;
            }
            else
            {
                _gridController.MergeObjectEvent -= OnMergeObjectEvent;
                _gridController.UnpackObjectEvent -= OnUnpackObjectEvent;
                _gridController.BirdMergedEvent -= OnBirdMergedEvent;
                _gridController.BirdSpawnedEvent -= OnBirdSpawnedEvent;
            }
        }

        private void OnMergeObjectEvent(Vector3 subgridObjectsPosition)
        {
            var particle = _vfxService.GetStopedParticle(ParticleType.Merge, out var particleData);
            particle.transform.position = subgridObjectsPosition + particleData.Offset;
            particle.Play();
        }

        private void OnUnpackObjectEvent(Vector3 subgridObjectsPosition)
        {
            var particle = _vfxService.GetStopedParticle(ParticleType.Unpack, out var particleData);
            particle.transform.position = subgridObjectsPosition + particleData.Offset;
            particle.Play();
        }

        private void OnBirdMergedEvent(Vector3 subgridObjectsPosition)
        {
            var particle = _vfxService.GetStopedParticle(ParticleType.BirdMerge, out var particleData);
            particle.transform.position = subgridObjectsPosition + particleData.Offset;
            particle.Play();
        }

        private void OnBirdSpawnedEvent(Transform parent)
        {
            var particle = _vfxService.GetStopedParticle(ParticleType.BirdStatic, out var particleData);
            particle.transform.SetParent(parent, false);
            particle.transform.position = parent.position + particleData.Offset;
            particle.Play();
        }
    }
}