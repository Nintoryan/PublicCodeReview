using System;
using System.Collections.Generic;
using VillageGame.Data;

namespace Web.ResponseStructs
{
    [Serializable]
    public class TodayTasksState
    {
        public string date;
        public List<DailyTask> tasks = new();
        public int snowdrift_amount = 20;
    }

    public class DailyTask
    {
        public ConditionData condition;
        public TaskState state;
    }

    public enum TaskState
    {
        none,
        in_progress,
        complited,
        close
    }
}