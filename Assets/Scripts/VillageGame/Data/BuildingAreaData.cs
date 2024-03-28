using System;
using UnityEngine;
using VillageGame.Data.Types;

namespace VillageGame.Data
{
    [Serializable]
    public class BuildingAreaData
    {
        public BoundsInt Area;
        public GameObject ClueArea;
        public BuildingType Type;
        public int BuildingId = -2;
        public bool IsBuildUp => BuildingId >= 0;

        public void SetBuild(int id)
        {
            Debug.Log($"Building {Type} ID Set:{id}  {ClueArea.gameObject.name}");
            BuildingId = id;
            ClueArea.SetActive(false);
        }

        public void ResetBuild(bool isShowHideClueAre = true)
        {
            BuildingId = -1;
            ClueArea.SetActive(isShowHideClueAre);
        }
    }
}