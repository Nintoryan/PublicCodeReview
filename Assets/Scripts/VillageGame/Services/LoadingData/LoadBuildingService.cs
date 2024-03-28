using System.Threading.Tasks;
using Util;
using VContainer;
using VillageGame.Infrastructure.Factories;
using VillageGame.Services.Buildings;
using VillageGame.Services.Storages;
using Web.RequestStructs;

namespace VillageGame.Services.LoadingData
{
    public class LoadBuildingService : ILoading
    {
        private readonly BuildingFactory _buildingFactory;
        private readonly BuildingAreaService _buildingAreaService;
        private readonly BuildingOnMapStorage _buildingOnMapStorage;
        private readonly TileAreasService _tileAreasService;


        public LoadBuildingService(IObjectResolver objectResolver)
        {
            _buildingFactory = objectResolver.Resolve<BuildingFactory>();
            _buildingAreaService = objectResolver.Resolve<BuildingAreaService>();
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _tileAreasService = objectResolver.Resolve<TileAreasService>();
        }


        public void Load(LoadData request)
        {
            var buildings = request.data.building;
            if (buildings != null)
            {
                for (int i = 0; i < buildings.Count; i++)
                {
                    if (_buildingAreaService.TryGetBuildingAreaByCoordinate(buildings[i].x, buildings[i].y, out var area))
                    {
                        var buildingPosition = _tileAreasService.CellToLocalPosition(area.Area.position);
                        var building = _buildingFactory.InstantiateBuildings(buildings[i].type, buildings[i].id, buildingPosition);
                        building.SetBuildData(buildings[i]);
                        _buildingOnMapStorage.LoadBuilding(building);
                        area.SetBuild(building.Data.ID);

                        Debugging.Log($"Building load: {building.Data.Type} {building.Data.ID} ");
                    }
                }
            }
        }
    }
}