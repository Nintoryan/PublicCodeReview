using System;

namespace Web.ResponseStructs
{
    [Serializable]
    public class EnergyData
    {
        public int energy;
        public DateTime time;

        public EnergyData(int energy, DateTime time)
        {
            this.energy = energy;
            this.time = time;
        }
    }


}