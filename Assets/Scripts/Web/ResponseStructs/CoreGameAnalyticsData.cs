using System;

namespace Web.ResponseStructs
{
    [Serializable]
    public class CoreGameAnalyticsData
    {
        public string request = "core_level_end";
        public CoreLevelEndData payload;
    }

    [Serializable]
    public class CoreLevelEndData
    {
        public bool victory;
        public int level;
    }
}
