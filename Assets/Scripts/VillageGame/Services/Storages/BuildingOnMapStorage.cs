using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.Logic.Buildings;
using Web.ResponseStructs.PayloadValues;

namespace VillageGame.Services.Storages
{
    public class BuildingOnMapStorage
    {
        private readonly PurchasedBuildingsStorage _purchasedBuildingStorage;
        public List<BuildData> BuildingsOnMapData => BuildingsOnMap.Select(b => b.BuildData).ToList();
        public List<Building> BuildingsOnMap { get; } = new();

        public Action<BuildingType, int> BuildBuildingEvent;

        public Action<BuildingType, int> RemoveBuildingEvent;

        public Action<BuildingType, int> MoveBuildingEvent;


        [Inject]
        public BuildingOnMapStorage(IObjectResolver objectResolver)
        {
        }

        public bool TryAddBuilding(Building building)
        {
            if (!IsCanBuildMore(building.Data.Type, building.Data.ID))
            {
                return false;
            }

            BuildingsOnMap.Add(building);

            if (!PurchasedBuildingsStorage.IsBuildingIsPurchased(building.Data.Type, building.Data.ID))
            {
                BuildBuildingEvent?.Invoke(building.Data.Type, building.Data.ID);
            }
            else
            {
                MoveBuildingEvent?.Invoke(building.Data.Type, building.Data.ID);
            }
         
            Debug.Log($"BuildingStorage: Add building {building.Data.Type} {building.Data.ID}." +
                      $"All Count = {BuildingsOnMap.Count} " +
                      $"This buildings count = {BuildingsOnMap.Count(b => b.Data.Type == building.Data.Type && b.Data.ID == building.Data.ID)}");
            return true;
        }

        public void LoadBuilding(Building building)
        {
            if (building.Data.Type == BuildingType.House && IsBuildUp(building))
            {
                return;
            }

            BuildingsOnMap.Add(building);
        }

        public void RefreshBuilding(Building building)
        {
            MoveBuildingEvent?.Invoke(building.Data.Type, building.Data.ID);
        }

        public void RemoveBuilding(Building building)
        {
            BuildingsOnMap.Remove(building);
            RemoveBuildingEvent?.Invoke(building.Data.Type, building.Data.ID);
            Debug.Log($"BuildingStorage: Remove building {building.Data.ID}. Count = {BuildingsOnMap.Count}");
        }

        #region Map buildings

        public bool IsBuildUp(Building building)
        {
            return BuildingsOnMap.Contains(building);
        }
        
        public bool IsBuildUp(BuildingType type, int id)
        {
            var building = BuildingsOnMap.FirstOrDefault(b => b.Data.ID == id && b.Data.Type == type);
            return building != null;
        }

        public Building GetBuilding(int x, int y)
        {
            return BuildingsOnMap.FirstOrDefault(b => b.BuildData.x == x && b.BuildData.y == y);
        }


        
        public bool TryGetBuilding(BuildingType type,int id, out Building building)
        {
            building = BuildingsOnMap.FirstOrDefault(b => b.Data.ID == id && b.Data.Type == type);
            return building != null;
        }


        public bool IsCanBuildMore(BuildingType type, int ID)
        {
            if (type == BuildingType.House && IsBuildUp(type, ID))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}