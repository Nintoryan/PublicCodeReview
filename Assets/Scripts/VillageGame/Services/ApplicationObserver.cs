using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VillageGame.Services.LoadingData;

namespace VillageGame.Services
{
    public class ApplicationObserver : MonoBehaviour
    {
        private LoadingService _loadingService;
        
        public Action SceneLoadEvent;
        public Action ApplicationQuitEvent;
        
        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _loadingService = resolver.Resolve<LoadingService>();
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }
  
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _loadingService.SceneLoad += SceneLoadInvoke;
            }
            else
            {
                _loadingService.SceneLoad -= SceneLoadInvoke;
            }
        }

        private void SceneLoadInvoke()
        {
            SceneLoadEvent?.Invoke();
        }

        private void OnApplicationQuit()
        {
            ApplicationQuitEvent?.Invoke();
        }
    }
}