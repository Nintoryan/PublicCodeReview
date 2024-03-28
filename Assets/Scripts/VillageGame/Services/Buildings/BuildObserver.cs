using System;
using UnityEngine;
using VContainer;
using VillageGame.Data.Types;

namespace VillageGame.Services.Buildings
{
    public class BuildObserver
    {
        public Action<BuildingType> StartBuildEvent;
        public Action StopBuildEvent;

        private bool _isStarting;
        
        [Inject]
        public BuildObserver(){}
        public void InvokeStartEvent(BuildingType type)
        {
            Debug.Log("Start Event Invoked");
            if (_isStarting)
            {
                return;
            }

            _isStarting = true;
            StartBuildEvent?.Invoke(type);
        }
        public void InvokeStopEvent()
        {
            _isStarting = false;
            StopBuildEvent?.Invoke();
        }
    }
}