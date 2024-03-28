using System;
using UnityEngine;
using VillageGame.Logic.Snowdrifts;

namespace VillageGame.Data
{
    [Serializable]
    public class ConstructionSiteData
    {
        public Vector2Int BuildAreaPosition;
        public ConstructionSite.StateType State;
        public ConstructionSite.PositionType PositionType;
        public int SpriteIndex;
    }
}