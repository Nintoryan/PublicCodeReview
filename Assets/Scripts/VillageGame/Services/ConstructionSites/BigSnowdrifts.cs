using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NavMeshPlus.Components;
using UnityEngine;
using Util;
using VillageGame.Services.Buildings;

namespace VillageGame.Services.Snowdrifts
{
    public class BigSnowdrifts : MonoBehaviour
    {
        private const float ANIMATION_DURATION = 6;

        [SerializeField] private NavMeshSurface _navMeshSurface;
        [SerializeField] private List<SpriteRenderer> _snowdrifts;
        [SerializeField] private BigSnowdriftParticleController _particleController;
        [SerializeField] private BuildingAreaService _buildingAreaService;
        [SerializeField] private GameObject _defaultRailway;
        [SerializeField] private GameObject _train;
        [SerializeField] private GameObject _enormousSnowDrift;
        [SerializeField] private GameObject _giantSnowMan;
        
        public bool IsActive { get; private set; } = true;
        private bool _isAnimation;

        public Action EndAnimationEvent;
        
        public void SetGiantSnowMan(bool isActive)
        {
            _giantSnowMan.SetActive(isActive);
            _giantSnowMan.SetActive(isActive);
        }

        public void SetSnowflakeFactoryCutsceneState(bool isActive)
        {
            Debugging.Log("!");
            _train.SetActive(!IsActive);
            _enormousSnowDrift.SetActive(isActive);
            _defaultRailway.SetActive(!isActive);
            foreach (var spriteRenderer in _snowdrifts)
            {
                spriteRenderer.gameObject.SetActive(isActive);
                spriteRenderer.sortingOrder =  isActive ? 1 : 0 ;
                spriteRenderer.color = Color.white;
            }
        }
        
        public void DisableSnowdrifts()
        {
            foreach (var spriteRenderer in _snowdrifts)
            {
                spriteRenderer.gameObject.SetActive(false);
            }

            IsActive = false;
       
        }

        public void PlayHideAnimation()
        {
            _particleController.StartAnimation();
            foreach (var spriteRenderer in _snowdrifts)
            {
                spriteRenderer.DOFade(0, ANIMATION_DURATION)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        spriteRenderer.gameObject.SetActive(false);
                        EndAnimationEvent?.Invoke();
                    })
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            }

            IsActive = false;
            StartCoroutine(UpdateNavMeshSurfaceWithDelay());
        }

        public void UpdateNavMesh()
        {
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
        }
        private IEnumerator UpdateNavMeshSurfaceWithDelay()
        {
            yield return new WaitForSeconds(ANIMATION_DURATION + 0.5f);
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
        }

        private void OnValidate()
        {
            if (_buildingAreaService == null)
            {
                _buildingAreaService = FindObjectOfType<BuildingAreaService>();
            }
        }
    }
}