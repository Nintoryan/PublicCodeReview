using System;
using System.Collections.Generic;
using SO;
using SO.Data;
using UnityEngine;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.Services.Storages;

namespace VillageGame.Services
{
    public class ProgressionService
    {
        private readonly ProgressionConfig _progressionConfig;
        private readonly RatingStorage _ratingStorage;
        private readonly BuildingOnMapStorage _buildingOnMapStorage;
        private readonly PurchasedBuildingsStorage _purchasedBuildingsStorage;

        public Action NewBuildingUnlockedEvent;
        public Action NewProjectUnlockedEvent;
        

        [Inject]
        public ProgressionService(IObjectResolver objectResolver)
        {
            _progressionConfig = objectResolver.Resolve<ProgressionConfig>();
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _purchasedBuildingsStorage = objectResolver.Resolve<PurchasedBuildingsStorage>();

            SubscribeToEvents(true);
        }

        ~ProgressionService() 
        {
            SubscribeToEvents(true);
        }

        private void SubscribeToEvents(bool flag)
        {
            if(flag)
            {
                _ratingStorage.ChangeLevel += OnChangeLevelEvent;
            }
            else
            {
                _ratingStorage.ChangeLevel -= OnChangeLevelEvent;
            }
        }

        private void OnChangeLevelEvent(int newLevel)
        {
            if (_progressionConfig.playerLevels[newLevel - 1].UnlockedBuildings.Length > 0)
            {
                Debug.Log("Building");
                NewBuildingUnlockedEvent?.Invoke();
            }

            if (_progressionConfig.playerLevels[newLevel -1].NewYearProjects.Length > 0)
            {
                Debug.Log("Project");
                NewProjectUnlockedEvent?.Invoke();
            }
        }

        public PlayerLevelData GetCurrentLevelData()
        {
            var level = _ratingStorage.Level > 0 ? _ratingStorage.Level - 1 :0;
            return  _progressionConfig.playerLevels[level ];
        }


        public AvailableBuildingsData GetAvailableBuildings(BuildingType type)
        {
            int playerLevel = _ratingStorage.Level;
            var availableBuildingIDs = GetAvailableBuildingIDs(type, playerLevel);
            var lockedBuildingIDs = GetLockedBuildingIDs(type, playerLevel);
            var purchasedBuildingIDs = GetPurchasedBuildingIDs(type);
            
            return new AvailableBuildingsData(availableBuildingIDs, lockedBuildingIDs, purchasedBuildingIDs);
        }

        public List<int> GetUnlockedNewYearProjects()
        {
            List<int> unlockedProjectsIDs = new();
            int playerLevel = _ratingStorage.Level;
            if (playerLevel < _progressionConfig.playerLevels.Length - 1)
            {
                for (int i = 0; i < playerLevel; i++)
                {
                    for (int j = 0; j < _progressionConfig.playerLevels[i].NewYearProjects.Length; j++)
                    {
                        unlockedProjectsIDs.Add(_progressionConfig.playerLevels[i].NewYearProjects[j].ID);
                    }
                }
            }
            return unlockedProjectsIDs;
        }

        private List<int> GetPurchasedBuildingIDs(BuildingType type)
        {
            return _purchasedBuildingsStorage.GetBuildingsIDS(type);
        }

        private List<int> GetLockedBuildingIDs(BuildingType type, int playerLevel)
        {
            List<int> lockedBuildingIDs = new();
            if (playerLevel < _progressionConfig.playerLevels.Length - 1)
            {
                for (int i = 0; i < _progressionConfig.playerLevels[playerLevel].UnlockedBuildings.Length; i++)
                {
                    if (_progressionConfig.playerLevels[playerLevel].UnlockedBuildings[i].Type == type)
                    {
                        lockedBuildingIDs.Add(_progressionConfig.playerLevels[playerLevel].UnlockedBuildings[i].ID);
                    }
                }
            }

            return lockedBuildingIDs;
        }

        private List<int> GetAvailableBuildingIDs(BuildingType type, int playerLevel)
        {
            List<int> availableBuildingIDs = new();
            for (int i = 0; i < playerLevel; i++)
            {
                foreach (var unlockBuilding in _progressionConfig.playerLevels[i].UnlockedBuildings)
                {
                    if ((unlockBuilding.Type != type) 
                        || (_buildingOnMapStorage.IsBuildUp(type,unlockBuilding.ID) && type == BuildingType.House)
                        || PurchasedBuildingsStorage.IsBuildingIsPurchased(type,unlockBuilding.ID))
                        continue;

                    availableBuildingIDs.Add(unlockBuilding.ID);
                }
            }

            return availableBuildingIDs;
        }
    }

    public class AvailableBuildingsData
    {
        public readonly List<int> AvailableBuildingIDs;
        public readonly List<int> LockedBuildingsIDs;
        public readonly List<int> PurchasedBuildingsIDs;
        
        public AvailableBuildingsData(List<int> availableBuildingIDs, List<int> lockedBuildingsIDs, List<int> purchasedBuildingsIDs)
        {
            AvailableBuildingIDs = availableBuildingIDs;
            LockedBuildingsIDs = lockedBuildingsIDs;
            PurchasedBuildingsIDs = purchasedBuildingsIDs;
        }
    }
}
