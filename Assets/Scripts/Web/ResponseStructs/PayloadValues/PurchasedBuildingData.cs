using System;
using VillageGame.Data.Types;

namespace Web.ResponseStructs.PayloadValues
{
    [Serializable]
    public class PurchasedBuildingData : IEquatable<PurchasedBuildingData>
    {
        public BuildingType Type;
        public int BuildingID;

        public bool Equals(PurchasedBuildingData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && BuildingID == other.BuildingID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PurchasedBuildingData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, BuildingID);
        }
    }
}