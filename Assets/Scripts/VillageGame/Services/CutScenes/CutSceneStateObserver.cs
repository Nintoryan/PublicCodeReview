using System;
using SO.Data;
using Util;

namespace VillageGame.Services.CutScenes
{
    public static class CutSceneStateObserver 
    {
        public static bool IsWatching { get; private set; }
        public static event Action<CutSceneData> StartCutsceneEvent;
        public static event Action<CutSceneData> EndCutsceneEvent;
        

        public static void InvokeStartEvent(CutSceneData cutSceneData)
        {
            Debugging.Log("Cut scene state observer: START");
            IsWatching = true;
            StartCutsceneEvent?.Invoke(cutSceneData);
        }

        public static void InvokeEndEvent(CutSceneData cutSceneData)
        {
            Debugging.Log("Cut scene state observer: END");
            IsWatching = false;
            EndCutsceneEvent?.Invoke(cutSceneData);
        }
        
    }
}