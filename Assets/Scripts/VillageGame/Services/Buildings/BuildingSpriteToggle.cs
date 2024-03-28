using UnityEngine;
using VContainer;
using VillageGame.Logic.Buildings;
using VillageGame.Logic.Tree;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;

namespace VillageGame.Services.Buildings
{
    public class BuildingSpriteToggle : MonoBehaviour
    {
        [SerializeField] private SpriteArrToggle _railwayStation;
        [SerializeField] private SpriteArrToggle _grandmaAndGrandpasHouse;
        
        private ConstructionSiteService _constructionSiteService;
        private BuildingOnMapStorage _buildingOnMapStorage;
        private CoreGameLoadService _coreGameLoad;
        private ChristmasTree _christmasTree;

        [Inject]
        public void Constructs(IObjectResolver objectResolver)
        {
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _coreGameLoad = objectResolver.Resolve<CoreGameLoadService>();
            _christmasTree = objectResolver.Resolve<ChristmasTree>();
            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();
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
                _coreGameLoad.LoadCoreGameSceneEvent += OnLoadCoreGameSceneEvent;
                _coreGameLoad.UnLoadCoreGameSceneEvent += OnCoreGameWinEvent;
            }
            else
            {
                _coreGameLoad.LoadCoreGameSceneEvent -= OnLoadCoreGameSceneEvent;
                _coreGameLoad.UnLoadCoreGameSceneEvent -= OnCoreGameWinEvent;
            }
        }

        private void OnCoreGameWinEvent()
        {
            foreach (var building in _buildingOnMapStorage.BuildingsOnMap)
            {
                building.ActiveSprite(true);
            }
            foreach (var building in _constructionSiteService.ConstructionSites)
            {
                building.ActiveSprite(true);
            }

            _christmasTree.ActiveSprite(true);
            _railwayStation.ActiveSprite(true);
            _grandmaAndGrandpasHouse.ActiveSprite(true);
        }

        private void OnLoadCoreGameSceneEvent()
        {
            foreach (var building in _buildingOnMapStorage.BuildingsOnMap)
            {
                building.ActiveSprite(false);
            }
            foreach (var building in _constructionSiteService.ConstructionSites)
            {
                building.ActiveSprite(false);
            }

            _christmasTree.ActiveSprite(false);
            _railwayStation.ActiveSprite(false);
            _grandmaAndGrandpasHouse.ActiveSprite(false);
        }
    }
}