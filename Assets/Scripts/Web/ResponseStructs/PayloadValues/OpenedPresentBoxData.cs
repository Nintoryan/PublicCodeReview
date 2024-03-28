using System;
using VillageGame.Data;

namespace Web.ResponseStructs.PayloadValues
{
    [Serializable]
    public class OpenedPresentBoxData
    {
        public int ID;
        public DayData Date;
        public bool IsOpen;
        public string StringValue;
    }
}