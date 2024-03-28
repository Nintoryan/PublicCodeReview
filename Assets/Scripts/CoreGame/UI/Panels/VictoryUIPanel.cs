using SO;
using System.Linq;
using Data.Scripts.Audio;
using UnityEngine;
using VContainer;

namespace CoreGame.UI.Panels
{
    public class VictoryUIPanel : EndGameUIPanel
    {
        [SerializeField] private LevelStorage _levelStorage;
        [SerializeField] private GameObject _nextLevelUIButton;

        private CutSceneConfig _cutSceneConfig;

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _cutSceneConfig = objectResolver.Resolve<CutSceneConfig>();
        }

        protected override void SubscribeToEvents(bool flag)
        {
            base.SubscribeToEvents(flag);
            if (flag)
            {
                _levelStorage.GameWonEvent += OnGameWonEvent;
            }
            else
            {
                _levelStorage.GameWonEvent -= OnGameWonEvent;
            }
        }

        protected override void OnEnergyUpdateEvent()
        {
            base.OnEnergyUpdateEvent();
            if (StaticPrefs.EnergyValue > 0)
            {
                _nextLevelUIButton.SetActive(true);
            }
        }


        private void OnGameWonEvent(int levelReward, int selectedLevel, bool isNextLevelButtonDeactivated)
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.WinCoreGame);
            _energyUIIndicator.UpdateValue(StaticPrefs.EnergyValue.ToString());
            if (isNextLevelButtonDeactivated)
            {
                _nextLevelUIButton.SetActive(false);
            }
            Show();
        }
    }
}
