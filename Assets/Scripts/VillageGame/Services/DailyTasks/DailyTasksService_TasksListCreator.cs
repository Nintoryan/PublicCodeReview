using System.Collections.Generic;
using System.Linq;
using SO;
using SO.Data;
using UnityEngine;
using Util;
using VillageGame.Data.Types;

namespace VillageGame.Services.DailyTasks
{
    public partial class DailyTasksService
    {
        private void TryAddRealLifeTask()
        {
            if (!IsCanAddNewTasks() || IsHasTypedTask(ConditionType.OrderPrice) || IsHasTypedTask(ConditionType.BuyProduct)) 
                return;

            var shuffledRealLifeTasks = _dailyTaskConfig.RealLifeTasks.Clone() as DailyTaskData[];
            shuffledRealLifeTasks.Shuffle();

            if (shuffledRealLifeTasks == null) return;
            
            foreach (var task in shuffledRealLifeTasks)
            {
                if(task == null) continue;
                
                //Не давать задание на бреслетик
                if(task.Condition.ValueString == "000D3A29FF6BA94811E8CBCA128EAB5E"
                   && task.Condition.Type == ConditionType.BuyProduct)
                    continue;
                
                //Не давать задание на денвич
                if(task.Condition.ValueString == "11ee52139ee3bd4abf28d7ad3a790510"
                   && task.Condition.Type == ConditionType.BuyProduct)
                    continue;
                
                var id = _dailyTaskConfig.GetRealLifeTaskID(task);
                
                if (!CompleteTrackingTasks.Contains(id.ToString()) && id != -1)
                {
                    switch (task.Condition.Type)
                    {
                        case ConditionType.OrderPrice:
                            RealLifeService.SetTrackOrderPrice(id,task.Condition.Value);
                            break;
                        case ConditionType.BuyProduct:
                            RealLifeService.SetTrackProductGuid(id,task.Condition.ValueString);
                            break;
                        default:
                            Debugging.Log("Something went really wrong, but I'll handle this.");    
                            return;
                    }
                    AllTodayTasks.Add(task);
                    return;
                }
            }
        }
        private void TryAddBuildingTask()
        {
            if (!IsCanAddNewTasks() || IsHasTypedTask(ConditionType.BuildHouse)) 
                return;
            
                var building = _progressionService.GetAvailableBuildings(BuildingType.House);

                if (building.AvailableBuildingIDs is not { Count: > 0 }) 
                    return;
            
            foreach (var buildingID in building.AvailableBuildingIDs)
            {
                var task = _dailyTaskConfig.GetTypedTask(ConditionType.BuildHouse, buildingID);
                
                if (task != null)
                {
                    AllTodayTasks.Add(task);
                    return;
                }
            }
        }

        private void TryAddDecorationTask()
        {
            if (!IsCanAddNewTasks() || IsHasTypedTask(ConditionType.BuildDecoration)) 
                return;
            
            var decorations = _progressionService.GetAvailableBuildings(BuildingType.Decoration);
            
            if(decorations.AvailableBuildingIDs is not {Count: > 0})
                return;

            foreach (var decorationID in decorations.AvailableBuildingIDs)
            {
                var task = _dailyTaskConfig.GetTypedTask(ConditionType.BuildDecoration, decorationID);

                if (task != null)
                {
                    AllTodayTasks.Add(task);
                    return;
                }
            }
        }
        
        private void TryAddLevelUpTask()
        {
            if (IsCanAddNewTasks() && !IsHasTypedTask(ConditionType.LevelUp))
            {
                var task = _dailyTaskConfig.GetTypedTask(ConditionType.LevelUp, _ratingStorage.Level + 1);
                if (task != null)
                {
                    AllTodayTasks.Add(task);
                }
            }
        }

        private void TryAddCoreGameWinLevelTask()
        {
            if (IsCanAddNewTasks() && !IsHasTypedTask(ConditionType.CoreGameWinLevel))
            {
                var task = _dailyTaskConfig.GetTypedTask(ConditionType.CoreGameWinLevel, StaticPrefs.SelectedCoreGameLevel + 1);
                if (task != null)
                {
                    AllTodayTasks.Add(task);
                }
            }
        }

        private void TryAddSnowdriftTask()
        {
            if (IsCanAddNewTasks() && !IsHasTypedTask(ConditionType.ClearSnowdrift))
            {
                var tasks = new List<DailyTaskData>();
                for (int i = 0; i < _constructionSiteService.GetSnowDriftSiteCount(); i++)
                {
                    var task = _dailyTaskConfig.GetTypedTask(ConditionType.ClearSnowdrift,i);
                    if (task != null)
                    {
                        tasks.Add(task);
                    }
                }

                if (tasks.Count > 0)
                {
                    AllTodayTasks.Add(tasks[Random.Range(0,tasks.Count)]);
                }
            }
        }

        private void TryAddRandomTasks()
        {
            var currentTypes = AllTodayTasks.Select(t => t.Condition.Type).ToList();
            var newTasks = _dailyTaskConfig.GetRandomTasks(currentTypes);
            if (newTasks == null)
            {
                return;
            }
            while (newTasks.Count > 0 && AllTodayTasks.Count < _dailyTaskConfig.MaxTasksCount )
            {
                var task = newTasks[0];

                if (task.Condition.Type == ConditionType.BuildHouse)
                {
                    var building = _progressionService.GetAvailableBuildings(BuildingType.House);

                    if (building.AvailableBuildingIDs == null || building.AvailableBuildingIDs.Count == 0)
                    {
                        newTasks.RemoveAt(0);
                        continue;
                    }
                }
                
                AllTodayTasks.Add(task);
                newTasks.RemoveAt(0);
            }
        }
    }
}