using System;
using UnityEngine;
using VillageGame.Data.Types;

namespace VillageGame.Data
{
    [Serializable]
    public class ConditionData: IEquatable<ConditionData>
    {
        public ConditionType Type;
        [Header("Если имеется в виду любое здание/уровень/декорация/количество побед," +
                "\nто указывается -1")]
        public int Value = -1;
        public string ValueString = "";
        
        public bool Equals(ConditionData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Value == other.Value && ValueString == other.ValueString;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ConditionData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, Value, ValueString);
        }
    }
}