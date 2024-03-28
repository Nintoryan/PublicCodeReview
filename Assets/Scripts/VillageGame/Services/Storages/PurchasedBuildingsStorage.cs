using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;
using Web.ResponseStructs.PayloadValues;

namespace VillageGame.Services.Storages
{
    public class PurchasedBuildingsStorage : ILoading
    {
        public static List<PurchasedBuildingData> PurchasedBuildings { get; private set; } = new();
        private readonly BuildingOnMapStorage _buildingsOnMapStorages;
        
        public Action ChangeEvent;
        public Action BuildingHasBeenAddedAsPresentEvent;
        
        [Inject]
        public PurchasedBuildingsStorage(IObjectResolver objectResolver)
        {
            _buildingsOnMapStorages = objectResolver.Resolve<BuildingOnMapStorage>();
        }
        
        public void Add(BuildingType type, int buildingID, bool isPresent = false)
        {
            if (IsCantAddThisBuilding(type, buildingID))
            {
                return;
            }

            PurchasedBuildings.Add(GetNewPurchasedBuildingData(type, buildingID));
            ChangeEvent?.Invoke();
            if (isPresent)
            {
                BuildingHasBeenAddedAsPresentEvent?.Invoke();
            }
        }
        
        public void Remove(BuildingType type, int buildingID)
        {
            if (IsBuildingIsPurchased(type, buildingID))
            {
                PurchasedBuildings.Remove(GetNewPurchasedBuildingData(type, buildingID));
                ChangeEvent?.Invoke();
            }
        }

        public bool TryRemove(BuildingType type, int buildingID)
        {
            if (IsBuildingIsPurchased(type, buildingID))
            {
                PurchasedBuildings.Remove(GetNewPurchasedBuildingData(type, buildingID));
                ChangeEvent?.Invoke();
                return true;
            }

            return false;
        }
        
        
        public int GetDecorationCount(int buildingID)
        {
            return PurchasedBuildings.Count(b => b.Type == BuildingType.Decoration && b.BuildingID == buildingID);
        }
        
        public List<int> GetBuildingsIDS(BuildingType type)
        {
            return PurchasedBuildings.Where(b => b.Type == type).Select(d => d.BuildingID).ToList();
        }

        public static bool IsBuildingIsPurchased(BuildingType type, int id)
        {
            return PurchasedBuildings.Count(b => b.Type == type && b.BuildingID == id) > 0;
        }

        private bool IsCantAddThisBuilding(BuildingType type, int buildingID)
        {
            return type == BuildingType.House 
                   && (IsBuildingIsPurchased(type, buildingID) || _buildingsOnMapStorages.IsBuildUp(type, buildingID));
        }

        private PurchasedBuildingData GetNewPurchasedBuildingData(BuildingType type, int buildingID)
        {
            return new PurchasedBuildingData()
            {
                Type = type,
                BuildingID = buildingID
            };
        }

        public void Load(LoadData request)
        {
            if (request.data.purchased_buildings != null)
            {
                PurchasedBuildings = request.data.purchased_buildings;
            }
        }
    }
}