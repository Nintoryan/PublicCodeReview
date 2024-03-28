using System;
using SO.Data.Characters;
using UnityEngine;
using VillageGame.Data;
using VillageGame.Services.CutScenes.CustomActions;

namespace SO.Data
{
    [CreateAssetMenu(fileName = "CutSceneData",  menuName = "SO/Data/CutSceneData")]
    public class CutSceneData: ScriptableObject
    {
        public int ID;
        public PresentBoxData PresentBox;
        public ConditionData Condition;
        
        [Space] 
        public bool CameraIsStayOnCurrentPosition;
        public bool CameraIsBackStartPos;
        public bool IsUnblockCharacters = true;
        public bool IsIgnoreCutSceneList;
        public bool IsCharacterTeleportToPosition = true;
        public CurtainParam CurtainParams;
        
        public CustomCutsceneActionType StartAction;
        [Space]
        [Header("При заполнении координат для персонажа убедитесь," +
                "\nчто координата является частью области NavMeshSurface =)" +
                "\n\nГлвный персонаж в диалоге - это первый элемент списка")]
        public CharacterPositionData[] CharacterData;
        public ReplicaData[] Replicas;
        public CustomCutsceneActionType EndAction;

        #region Editor

        public void RefreshZoomForAllReplicas(float zoom)
        {
            foreach (var replicaData in Replicas)
            {
                replicaData.Camera.Zoom = zoom;
            }   
        }

        #endregion
        
        [Serializable]
      public class CurtainParam
      {
          public bool IsShowOnStart;
          [TextArea]public string Text;
      }
    }


}