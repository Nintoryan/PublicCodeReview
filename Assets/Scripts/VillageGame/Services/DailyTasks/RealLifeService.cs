using System;
using Util;

namespace VillageGame.Services.DailyTasks
{
    public class RealLifeService
    {
        public static event Action<int,int> TrackOrderPrice; 
        public static event Action<int,string> TrackProductGuid;

        public static event Action<int> TaskDone; 

        public static void SetTrackOrderPrice(int taskID, int price)
        {
            TrackOrderPrice?.Invoke(taskID,price);
        }

        public static void SetTrackProductGuid(int taskID, string productGuid)
        {
            TrackProductGuid?.Invoke(taskID,productGuid);
        }

        public static void TaskDoneResponse(string taskID)
        {
            var id = -404;
            try
            {
                id = Convert.ToInt32(taskID);
            }
            catch (Exception e)
            {
                Debugging.Log($"Task ID convert went wrong! Attempted to convert:{taskID}");
            }
            
            if(id != -404)
                TaskDone?.Invoke(id);
        }
    }
}