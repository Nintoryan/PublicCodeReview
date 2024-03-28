using System.Collections.Generic;
using SO.Data;
using UnityEngine;
using Util;
using VContainer;
using Web.Api;

namespace VillageGame.Services.CutScenes
{
    public class CutSceneEditor: MonoBehaviour
    {
        [SerializeField] private List<CutSceneData> _list;

        [Space]
        [SerializeField] private float _zoom;


        public void SetZoom()
        {
            foreach (var cutscene in _list)
            {
                cutscene.RefreshZoomForAllReplicas(_zoom);
            }
        }


        public void GetTime()
        {
            Debugging.Log("Get time");
            WebAPI.GetServerTime();
        }
    }
}