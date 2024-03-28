using System;
using UnityEngine;
using VillageGame.Data.Types;
using VillageGame.Logic.Curtains;
using VillageGame.Logic.Tutorial;
using VillageGame.Services.CutScenes;
using VillageGame.Services.CutScenes.CustomActions;

namespace Util
{
    public static class Constance
    {
        public static int MAX_ENERGY; //устанавливаеться из конфига

        public const string CHARACTER_ROOT_TAG = "CharactersRoot";
        public const string CHARACTER_INITIAL_POINT_TAG = "CharactersInitialPoint";

        public static bool IsWatchingCinema()
        {
            return CloudCurtain.IsAnimating 
                   || CutSceneActionsExecutor.IsWatching 
                   || CutSceneStateObserver.IsWatching;
        }
        
        public static Vector3 GetBuildingOffset(BuildingType buildingType)
        {
            switch (buildingType)
            {
                default:
                case BuildingType.None:
                case BuildingType.House:
                    return Vector3.zero;
                case BuildingType.Decoration:
                    return  new Vector3(0, -1.2F, 0);;
            }
        }
    }
}