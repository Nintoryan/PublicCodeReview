using System;

namespace Web.ResponseStructs
{
    [Serializable]
    public class IceCubesData
    {
        public bool active;
        public bool[] cleared_cubes;

        public IceCubesData(bool active, bool[] cleared_cubes)
        {
            this.active = active;
            this.cleared_cubes = cleared_cubes;
        }
    }
}
