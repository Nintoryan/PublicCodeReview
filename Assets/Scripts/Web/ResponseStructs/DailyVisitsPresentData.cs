using System;

namespace Web.ResponseStructs
{
    [Serializable]
    public class DailyVisitsPresentData
    {
        public int visit_number;
        public int next_present_number;
        public bool is_open_today;
    }
}