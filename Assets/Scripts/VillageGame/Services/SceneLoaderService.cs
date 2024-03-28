using System;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace VillageGame.Services
{
    public class SceneLoaderService: IInitializable
    {
        public Action<Scene> SceneUnloadedEvent;

        [Inject]
        public SceneLoaderService()
        {
            
        }
        public void Initialize()
        {
            SubsribeToEvents(true);
        }

        ~SceneLoaderService()
        {
            SubsribeToEvents(false);
        }
        
        private void SubsribeToEvents(bool flag)
        {
            if (flag)
            {
                SceneManager.sceneUnloaded += OnSceneUnloaded;
            }
            else
            {
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
            }
        }
        
        
        private void OnSceneUnloaded(Scene scene)
        {
            SceneUnloadedEvent?.Invoke(scene);
        }
        
        public void LoadSceneAdditive(int id)
        {
            SceneManager.LoadScene(id, LoadSceneMode.Additive);
        }
    }
}