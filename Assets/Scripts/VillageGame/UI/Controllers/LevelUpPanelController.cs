using System.Collections;
using SO;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Services.Storages;
using VillageGame.UI.Panels;
using VillageGame.UI.Services;

namespace VillageGame.UI.Controllers
{
    public class LevelUpPanelController : IInitializable
    {
        private readonly ProgressionConfig _progressionConfig;
        private readonly RatingStorage _ratingStorage;
        private readonly LevelUpPanel _levelUpPanel;
        private readonly CoroutineRunner _coroutineRunner;
        
        [Inject]
        public LevelUpPanelController(IObjectResolver resolver)
        {
            _progressionConfig = resolver.Resolve<ProgressionConfig>();
            _ratingStorage = resolver.Resolve<RatingStorage>();
            _levelUpPanel = resolver.Resolve<UIFacade>().FindPanel<LevelUpPanel>();
            _coroutineRunner = resolver.Resolve<CoroutineRunner>();

        }
        
        public void Initialize()
        {
            SubscribeToEvents(true);
        }
        
        ~LevelUpPanelController()
        {
            SubscribeToEvents(false);
        }

        

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _ratingStorage.ChangeLevel += OnChangeLevel;
            }
            else
            {
                _ratingStorage.ChangeLevel -= OnChangeLevel;
            }
        }

        private void OnChangeLevel(int newLevel)
        {
            _coroutineRunner.StartCoroutine(ShowOnFree(newLevel));
        }

        private IEnumerator ShowOnFree(int newLevel)
        {
            yield return new WaitForSecondsRealtime(0.55f);
            if (UIWindowObserver.CurrentOpenedPanel is not null)
            {
                yield return new WaitUntil(() => UIWindowObserver.CurrentOpenedPanel is null);
                _levelUpPanel.Show();
                _levelUpPanel.SetData(newLevel,_progressionConfig.playerLevels[newLevel-1]); 
            }
            else
            {
                _levelUpPanel.Show();
                _levelUpPanel.SetData(newLevel,_progressionConfig.playerLevels[newLevel-1]); 
            }
        }
    }
}