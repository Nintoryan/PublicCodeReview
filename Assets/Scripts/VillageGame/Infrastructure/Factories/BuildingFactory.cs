using SO;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Logic.Buildings;
using VillageGame.Logic.Snowdrifts;
using VillageGame.Services.Buildings;
using VillageGame.UI;
using Object = UnityEngine.Object;

namespace VillageGame.Infrastructure.Factories
{
    public class BuildingFactory
    {
        private readonly BuildingsConfig _buildingsConfig;
        private readonly InputBlockService _inputBlock;
        private readonly Transform _buildingsRoot;
        private readonly TileAreasService _tileAreaService;
        private const string _buildingsRootTag = "BuildingsRoot";

        [Inject]
        public BuildingFactory(IObjectResolver objectResolver)
        {
            _buildingsRoot = GameObject.FindWithTag(_buildingsRootTag).transform;
            _buildingsConfig = objectResolver.Resolve<BuildingsConfig>();
            _inputBlock = objectResolver.Resolve<InputBlockService>();
            _tileAreaService = objectResolver.Resolve<TileAreasService>();
        }

        public Building InstantiateBuildings(BuildingType type,int id, Vector3 position)
        {
            var buildingData = _buildingsConfig.GetData(type,id);
            if (buildingData == null)
            {
                Debug.LogWarning($"BuildingFactory: BuildingData by id {id} not find");
                return null;
            }

            var building = Object.Instantiate(buildingData.Prefab, _buildingsRoot);
            building.transform.position = position + Constance.GetBuildingOffset(type);
            building.Initial(buildingData, _inputBlock);

            return building;
        }



        public ConstructionSite InstantiateConstructionSite(Vector3Int buildingAreaPosition, Transform root)
        {
            var site = Object.Instantiate(_buildingsConfig.ConstructionSitePrefab, root);

            var position = _tileAreaService.CellToLocalPosition(buildingAreaPosition);
            site.transform.position = position;
            var sprite = _buildingsConfig.GetSnowdriftSprite(out var spriteIndex);

            ConstructionSiteData data = new ConstructionSiteData()
            {
                BuildAreaPosition = new Vector2Int(buildingAreaPosition.x, buildingAreaPosition.y),
                PositionType = Mathf.Abs(buildingAreaPosition.x) > 11 || Mathf.Abs(buildingAreaPosition.y) > 11  
                    ? ConstructionSite.PositionType.Distant
                    : ConstructionSite.PositionType.Nearby,
                SpriteIndex = spriteIndex,
                State = ConstructionSite.StateType.Snowdrift
            };

            site.gameObject.name += $"_{data.PositionType}_{data.BuildAreaPosition}";
            site.SetData(data);
            site.SetSprite(sprite);
            return site;
        }

        public ConstructionSite InstantiateConstructionSite(ConstructionSiteData data, Transform root)
        {
            var site = Object.Instantiate(_buildingsConfig.ConstructionSitePrefab, root);

            var position = _tileAreaService.CellToLocalPosition((Vector3Int)data.BuildAreaPosition);
            site.transform.position = position;

            Sprite sprite = null;
            switch (data.State)
            {
                case ConstructionSite.StateType.None:
                default:
                    break;
                case ConstructionSite.StateType.Snowdrift:
                    sprite = _buildingsConfig.GetSnowdriftSprite(data.SpriteIndex);
                    break;
                case ConstructionSite.StateType.ConstructionSite:
                    sprite =_buildingsConfig.ConstructionSiteSprite;;
                    break;
            }
            
            site.gameObject.name += $"_{data.PositionType}_{data.BuildAreaPosition}";
            site.SetData(data);
            site.SetSprite(sprite);
            return site;
        }
    }
}