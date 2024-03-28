using System;
using System.Linq;
using System.Threading.Tasks;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Storages;
using Web.RequestStructs;

namespace VillageGame.Services.Buildings
{
    public class MainerService : ILoading
    {
        private readonly BuildingOnMapStorage _buildingOnMapStorage;
        public Action<MainerData> MainerTickEvent;

        [Inject]
        public MainerService(IObjectResolver objectResolver)
        {
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
        }

        ~MainerService()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _buildingOnMapStorage.BuildBuildingEvent += OnBuildHouse;
                _buildingOnMapStorage.RemoveBuildingEvent += OnRemoveHouse;
                foreach (var building in _buildingOnMapStorage.BuildingsOnMap)
                {
                    building.Mainer.TickEvent += OnTick;
                }
            }
            else
            {
                _buildingOnMapStorage.BuildBuildingEvent -= OnBuildHouse;
                _buildingOnMapStorage.RemoveBuildingEvent -= OnRemoveHouse;
                foreach (var building in _buildingOnMapStorage.BuildingsOnMap.Where(building => building != null))
                {
                    building.Mainer.TickEvent -= OnTick;
                }
            }
        }

        private void OnRemoveHouse(BuildingType type,int id)
        {
            if (type == BuildingType.House &&  _buildingOnMapStorage.TryGetBuilding(type,id, out var building))
            {
                if (building.Data.MainerData.CooldownMinutes == 0)
                {
                    return;
                }
                
                building.Mainer.TickEvent -= OnTick;
            }
        }

        private void OnBuildHouse(BuildingType type,int id)
        {
            if (type == BuildingType.House &&  _buildingOnMapStorage.TryGetBuilding(type,id, out var building))
            {
                if (building.Data.MainerData.CooldownMinutes == 0)
                {
                    return;
                }
                
                building.StartUpdateMainer();
                building.Mainer.TickEvent += OnTick;
            }
        }

        private void OnTick(MainerData mainerData)
        {
            MainerTickEvent?.Invoke(mainerData);
        }

        public void Load(LoadData request)
        {
            SubscribeToEvents(true);
            
            
            if (Extensions.TryParseDate(request.data.exit_time, out var exitTime))
            {
                foreach (var building in _buildingOnMapStorage.BuildingsOnMap)
                {
                    if (building.Data.MainerData.CooldownMinutes == 0)
                    {
                        return;
                    }
                    
                    building.Mainer.TickEvent += OnTick;
                    
                    if (Extensions.TryParseDate(building.BuildData.build_time , out var buildTime))
                    {
                        building.Mainer.Load(lastExitTime: exitTime, buildTime, request.time);
                    }

                    building.StartUpdateMainer();
                }
            }
        }
        
    }
}