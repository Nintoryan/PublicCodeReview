using System;

namespace Web.ResponseStructs
{
    [Serializable]
    public class WinNumberData
    {
        public string day;
        public int all_wins;
        public int wins_in_row;
    }
}