using CoreGame;
using SO;
using System.Collections;
using UnityEngine;
using VContainer;
using VillageGame.Logic.Snowdrifts;
using VillageGame.Services.Buildings;
using VillageGame.Services.Snowdrifts;

namespace VillageGame.Services
{
    public class VillageVfxAdapter : MonoBehaviour
    {
        [SerializeField] private Transform _vfxRoot;
        private VFXService _vfxService;

        private ConstructionSiteService _constructionSiteService;
        private NewYearProjectsService _newYearProjectsService;

        [SerializeField] private ParticleSystem _lanternProjectParticle;
        private BuildService _buildService;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            var vfxConfig = objectResolver.Resolve<VFXConfig>();
            _vfxService = new VFXService(_vfxRoot, vfxConfig);

            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();
            _buildService = objectResolver.Resolve<BuildService>();
            _newYearProjectsService = objectResolver.Resolve<NewYearProjectsService>();
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
                _constructionSiteService.ClearSnowdriftEvent += ClearSnowdriftEvent;
                _buildService.BuildingPlaceEvent += BuildBuildingEvent;
                _newYearProjectsService.LanternProjectSettedEvent += OnLanternProjectSettedEvent;
            }
            else
            {
                _constructionSiteService.ClearSnowdriftEvent -= ClearSnowdriftEvent;
                _buildService.BuildingPlaceEvent -= BuildBuildingEvent;
                _newYearProjectsService.LanternProjectSettedEvent -= OnLanternProjectSettedEvent;
            }
        }

        private void BuildBuildingEvent(Vector3 position)
        {
            var particle = _vfxService.GetStopedParticle(ParticleType.BuildComplete, out var particleData);
            particle.transform.position = position + particleData.Offset;
            StartCoroutine(PlayWithDelay(particle, 0.5f));
        }

        private void ClearSnowdriftEvent(ConstructionSite obj)
        {
            var particle = _vfxService.GetStopedParticle(ParticleType.ClearSnowdrift, out var particleData);
            particle.transform.position = obj.transform.position + particleData.Offset;
            particle.Play();
        }


        private void OnLanternProjectSettedEvent(bool isActive)
        {
            if (isActive)
            {
                _lanternProjectParticle.Play();
            }
            else
            {
                _lanternProjectParticle.Stop();
            }
        }

        private IEnumerator PlayWithDelay(ParticleSystem particle, float delay)
        {
            yield return new WaitForSeconds(delay);
            particle.Play();
        }
    }
}