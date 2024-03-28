using System;
using SO.Data;

namespace VillageGame.Data
{
    [Serializable]
    public class DayDailyTasksData
    {
        public string Id;
        public DayData Day;
        public DailyTaskData[] Tasks;
    }
}