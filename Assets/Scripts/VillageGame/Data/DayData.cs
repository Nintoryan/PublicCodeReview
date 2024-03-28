using System;

namespace VillageGame.Data
{
    [Serializable]
    public class DayData: IEquatable<DayData>
    {
        public int Day;
        public int Month;
        public int Year;
        public bool IsEmpty => Day == 0 && Month == 0 && Year == 0;

        public DateTime GetData()
        {
            return new DateTime(Year, Month, Day);
        }

        
        public string GetDateText()
        {
            return $"{Day:00}.{Month:00}";
        }

        public string GetFullDateText()
        {
            var year = Year > 10 ? Year.ToString()[^2..] : Year.ToString();
            return $"{Day:00}.{Month:00}.{year}";
        }

        public string GetFullYearDateText()
        {
            return $"{Day:00}.{Month:00}.{Year}";
        }

        public bool Equals(DayData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Day == other.Day && Month == other.Month && Year == other.Year;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DayData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Day, Month, Year);
        }
    }
}