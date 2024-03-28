using System;

namespace VillageGame.Services
{
    public static class WherePresentsService
    {
        public static Action WherePresentsWatched;
        public static Action YearPizzaInfoWatched;

        public static void Watch_WherePresents()
        {
            WherePresentsWatched?.Invoke();
        }

        public static void Watch_YearPizzaInfo()
        {
            YearPizzaInfoWatched?.Invoke();
        }
        
    }
}