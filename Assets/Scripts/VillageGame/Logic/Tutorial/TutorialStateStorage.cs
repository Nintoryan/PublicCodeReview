using System;
using System.Linq;
using System.Threading.Tasks;
using Util;
using VContainer;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;

namespace VillageGame.Logic.Tutorial
{
    public class TutorialStateStorage : ILoading
    {
        private const int MAX_TUTORIAL_INDEX = 4;
        public VillageTutorialData[] TutorialData { get; private set; }

        public event Action DataLoaded;
        public event Action RefreshTutorialsDataEvent;
        public event Action EndWatchLastTutorialEvent;
        public event Action<int> StartWatchTutorialEvent;
        [Inject]public TutorialStateStorage(){}

        public void InvokeStartTutorialWatching(int tutorialID)
        {
            StartWatchTutorialEvent?.Invoke(tutorialID);    
        }
        
        public void InvokeEndTutorialWatching(int tutorialID)
        {
            var tutorial = TutorialData.FirstOrDefault(t => t.ID == tutorialID);
            if (tutorial != null)
            {
                tutorial.IsWatched = true;

                RefreshTutorialsDataEvent?.Invoke();
                if (tutorialID == MAX_TUTORIAL_INDEX)
                {
                    EndWatchLastTutorialEvent?.Invoke();
                }
            }
        }

        public void Load(LoadData request)
        {
            if (request.data.village_tutorials == null)
            {
                TutorialData = new VillageTutorialData[]
                {
                    new() { ID = 0, IsWatched = false },
                    new() { ID = 1, IsWatched = false },
                    new() { ID = 2, IsWatched = false },
                    new() { ID = 3, IsWatched = false },
                    new() { ID = 4, IsWatched = false }
                };
            }
            else
            {
                TutorialData = request.data.village_tutorials;
            }
            DataLoaded?.Invoke();
        }
    }
}