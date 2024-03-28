using System.Collections.Generic;
using System.Linq;
using SO.Data;
using UnityEngine;
using Util;
using VillageGame.Data;
using VillageGame.Data.Types;
using Web.ResponseStructs;

namespace SO
{
    [CreateAssetMenu(fileName = "DailyTasksConfig", menuName = "SO/DailyTasksConfig")]
    public class DailyTasksConfig : ScriptableObject
    {
        public int MaxTasksCount = 5;

        [Header("Задания в реальной жизни")] public DailyTaskData[] RealLifeTasks;

        [Space, Header("\nЗадания, в которых важна текущая прогрессия игрока.")]
        public DailyTaskData[] BuildHouseTasks;

        public DailyTaskData[] BuildDecorationTasks;
        public DailyTaskData[] LevelUpTasks;
        public DailyTaskData[] CoreGameWinLevelTasks;
        public DailyTaskData[] SnowDriftTasks;

        [Space, Header("\nЗадания, которыми будет заполняться нехватка заданий.")]
        public DailyTaskData[] RandomTasks;


        public DailyTaskData GetTypedTask(ConditionType type, int value, string valueString = "")
        {
            DailyTaskData result = null;
            switch (type)
            {
                case ConditionType.BuildHouse:
                    result = BuildHouseTasks.FirstOrDefault(t => t.Condition.Value == value);
                    break;

                case ConditionType.BuildDecoration:
                    result = BuildDecorationTasks.FirstOrDefault(t => t.Condition.Value == value);
                    break;

                case ConditionType.ClearSnowdrift:
                    return SnowDriftTasks.FirstOrDefault(t => t.Condition.Type == type && t.Condition.Value == value);

                case ConditionType.LevelUp:
                    return LevelUpTasks.FirstOrDefault(t => t.Condition.Value >= value && t.Condition.Value <= value + 3);

                case ConditionType.CoreGameWinLevel:
                    return CoreGameWinLevelTasks.FirstOrDefault(t =>
                        t.Condition.Value >= value && t.Condition.Value <= value + 5);

                case ConditionType.CoreGameNumberWinInARow:
                    break;

                case ConditionType.CoreGameNumberWin:
                    break;
                case ConditionType.BuyProduct:
                case ConditionType.OrderPrice:
                    return RealLifeTasks.FirstOrDefault(t =>
                        t.Condition.Value == value && t.Condition.ValueString == valueString);

                case ConditionType.None:
                default:
                    return null;
            }

            if (result == null)
            {
                result = RandomTasks.FirstOrDefault(t => t.Condition.Type == type && t.Condition.Value == value);
            }

            return result;
        }

        public List<DailyTaskData> GetRandomTasks(List<ConditionType> currentTasks)
        {
            var uniqueTasks = RandomTasks
                .Where(t => !currentTasks.Contains(t.Condition.Type))
                .GroupBy(t => t.Condition.Type)
                .Select(group => group.ToList());

            var selectedTasks = uniqueTasks
                .Select(group => group[Random.Range(0, group.Count)])
                .ToList();

            return selectedTasks;
        }


        public List<DailyTaskData> GetTasks(TodayTasksState _todayTasksState)
        {
            var allCondition = _todayTasksState.tasks.Select(t => t.condition);
            var tasks = new List<DailyTaskData>();

            foreach (var conditionData in allCondition)
            {
                var task = GetTypedTask(conditionData.Type, conditionData.Value, conditionData.ValueString);
                if (task)
                {
                    tasks.Add(task);
                }
                else
                {
                    Debugging.Log(
                        $"DailyTasksConfig: GetTasks -> cannot find {conditionData.Type} {conditionData.Value}",
                        ColorType.Magenta, LogStyle.Warning);
                }
            }

            Debugging.Log(
                $"DailyTasksConfig tasks state count {_todayTasksState.tasks.Count} return count = {tasks.Count}",
                ColorType.Magenta);
            return tasks;
        }

        public int GetRealLifeTaskID(DailyTaskData realLifeTask)
        {
            if (realLifeTask.Condition.Type is not ConditionType.OrderPrice and not ConditionType.BuyProduct)
            {
                Debugging.Log("Can't get id of not real life task");
                return -1;
            }

            for (int i = 0; i < RealLifeTasks.Length; i++)
            {
                if (RealLifeTasks[i].Condition.Value == realLifeTask.Condition.Value &&
                    RealLifeTasks[i].Condition.ValueString == realLifeTask.Condition.ValueString)
                {
                    return i;
                }
            }

            Debugging.Log("Real life task not in the list? Can't find it");
            return -1;
        }

        public int GetRealLifeTaskID(DailyTask realLifeTask)
        {
            if (realLifeTask.condition.Type is not ConditionType.OrderPrice and not ConditionType.BuyProduct)
            {
                Debugging.Log("Can't get id of not real life task");
                return -1;
            }

            for (int i = 0; i < RealLifeTasks.Length; i++)
            {
                if (RealLifeTasks[i].Condition.Equals(realLifeTask.condition))
                {
                    return i;
                }
            }

            Debugging.Log("Real life task not in the list? Can't find it");
            return -1;
        }
    }
}