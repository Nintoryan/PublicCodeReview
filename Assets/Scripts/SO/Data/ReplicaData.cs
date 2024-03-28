using System;
using SO.Data.Characters;
using UnityEngine;
using VillageGame.Services.CutScenes.CustomActions;

namespace SO.Data
{
    [Serializable]
    public class ReplicaData
    {
        public CharacterType Character;
        public CharacterReaction CharacterReaction;
        public PortraitReaction PortraitReaction;
        public CustomCutsceneActionType CustomAction;
        public CameraParam Camera;
        [TextArea] public string Text;
        
        [Serializable]
        public class CameraParam
        {
            public int PositionByBuildingID = -1;
            [Header("Если Building ID равен -1, то камера будет ориентироваться по Camera Position")]
            public Vector2 Position;
            public float Zoom;
            public bool IsIceBlock;
        }
    }
}