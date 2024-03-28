using System.Linq;
using SO.Data;
using UnityEngine;
using Util;
using VillageGame.Data;
using VillageGame.Data.Types;
using Web.ResponseStructs;

namespace VillageGame.Services.DailyTasks
{
    public partial class DailyTasksService
    {
        private bool IsHasThisTasks(ConditionData condition, out DailyTaskData task)
        {
            if (AllTodayTasks == null)
            {
                task = null;
                return false;
            }

            var allConditions = AllTodayTasks.Select(t => t?.Condition);

            var universalCondition = allConditions.FirstOrDefault(c => c?.Type == condition.Type && c.Value == -1);
            if (universalCondition != null)
            {
                task = AllTodayTasks.FirstOrDefault(t => t != null && t.Condition.Equals(universalCondition));
                return true;
            }

            task = AllTodayTasks.FirstOrDefault(t => t != null && t.Condition.Equals(condition));
            return task != null;
        }

        private bool IsCanAddNewTasks()
        {
            if (AllTodayTasks == null) return true;
            return AllTodayTasks.Count < _dailyTaskConfig.MaxTasksCount;
        }

        private bool IsHasTypedTask(ConditionType conditionType)
        {
            if (AllTodayTasks == null) return true;
            return AllTodayTasks.Any(t => t.Condition.Type == conditionType);
        }

        public TaskState GetTaskState(ConditionData condition)
        {
            Debugging.Log($"GetTaskState condition {condition.Type} {condition.Value}",ColorType.Red);
            var universalTask = TodayTasksState.tasks.FirstOrDefault(t => t.condition.Equals(new ConditionData()
            {
                Type = condition.Type,
                Value = -1
            }));

            if (universalTask == null)
            {
                var task = TodayTasksState.tasks.FirstOrDefault(t => t.condition.Equals(condition));
                Debugging.Log($"universal tasks == null true || tasks == null {task == null}  {task?.state} ",ColorType.Red);
                return task?.state ?? TaskState.none;
            }

            return universalTask.state;
        }
    }
}