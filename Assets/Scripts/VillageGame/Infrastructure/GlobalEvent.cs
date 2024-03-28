using System;
using SO.Data;

namespace DefaultNamespace.VillageGame.Infrastructure
{
    public  static class GlobalEvent
    {
        public static Action LoadGameEvent;
        public static void LoadGameInvoke() => LoadGameEvent?.Invoke();
        
        public static Action LoadCoreGameSceneEvent;
        public static void LoadCoreGameSceneInvoke() => LoadCoreGameSceneEvent?.Invoke();

        
        public static Action LoadVillageSceneEvent;
        public static void LoadVillageSceneInvoke() => LoadVillageSceneEvent?.Invoke();
    }
}