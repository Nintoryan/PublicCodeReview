using System;
using VillageGame.Data.Types;

namespace Web.ResponseStructs.PayloadValues
{
    [Serializable]
    public struct BuildData : IEquatable<BuildData>
    {
        public string build_time;
        public int id;
        public int x;
        public int y;
        public BuildingType type;

        public bool Equals(BuildData other)
        {
            return build_time == other.build_time && id == other.id && x == other.x && y == other.y && type == other.type;
        }

        public override bool Equals(object obj)
        {
            return obj is BuildData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(build_time, id, x, y, (int)type);
        }
    }
}