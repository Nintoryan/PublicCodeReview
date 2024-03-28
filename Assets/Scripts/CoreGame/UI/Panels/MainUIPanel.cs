using CoreGame.UI.Indicators;
using Data.Scripts.Audio;
using DG.Tweening;
using SO;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VillageGame.Services;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;

namespace CoreGame.UI.Panels
{
    public class MainUIPanel : UIPanel
    {
        [SerializeField] private LevelStorage _levelStorage;
        [SerializeField] private LevelTimer _levelTimer;
        [SerializeField] private Transform _endgameTimerUIPanel;
        [SerializeField] private Transform _endgameTimerTarget;
        [SerializeField] private SceneLoader _sceneUnloader;
        private DodoBirdsService _dodoBirdsService;

        [FormerlySerializedAs("_endGameTimerUIText")]
        [SerializeField] private TopPanel _topPanel;

        [SerializeField] private HintModalUIPanel _hintModalUIPanel;
        [SerializeField] private ShuffleModalUIPanel _shuffleModalUIPanel;
        [SerializeField] private DodoBirdUIIndicator _dodoBirdCounter;
        [SerializeField] private EndGameUIPanel _endGameVictoryPanel;
        [SerializeField] private EndGameUIPanel _endGameDefeatPanel;
        [SerializeField] private EventButton _restartButton;
        [SerializeField] private RestartModalUIPanel _restartModalUIPanel;

        private bool _isGameWin = true;

        [Inject]
        public void Contruct(IObjectResolver objectResolver)
        {
            _dodoBirdsService = objectResolver.Resolve<DodoBirdsService>();
        }


        private void OnEnable()
        {
            SubscribeToEvents(true);
            _dodoBirdCounter.Hide();
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _levelTimer.TimerUdpatedEvent += OnTimerUpdatedEvent;
                _levelTimer.TimerDeactivatedEvent += OnTimerDeactivatedEvent;
                _levelTimer.TimerEndedEvent += OnTimerEndedEvent;
                _restartButton.ClickEvent += OnRestartButtonClickEvent;
            }
            else
            {
                _levelTimer.TimerUdpatedEvent -= OnTimerUpdatedEvent;
                _levelTimer.TimerDeactivatedEvent -= OnTimerDeactivatedEvent;
                _levelTimer.TimerEndedEvent -= OnTimerEndedEvent;
                _restartButton.ClickEvent -= OnRestartButtonClickEvent;
            }
        }

        private void OnRestartButtonClickEvent()
        {
            _restartModalUIPanel.Show();
        }

        private void OnTimerEndedEvent()
        {
            _isGameWin = false;
        }

        public void EndGame()
        {
            _sceneUnloader.UnloadScene(false);
        }

        public void ShuffleLevel()
        {
            if (StaticPrefs.CoreGameShuffles >= 1)
            {
                if (_levelStorage.ShuffleObjects())
                {
                    StaticPrefs.CoreGameShuffles--;
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameShuffle);
                }
            }
            else
            {
                if (StaticPrefs.SelectedCoreGameLevel == 2)
                {
                    _levelStorage.ShuffleObjects();
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameShuffle);
                }
                else
                {
                    _shuffleModalUIPanel.Show();
                }
            }
        }

        public void ShowClue()
        {
            if (StaticPrefs.CoreGameHints >= 1)
            {
                if (_levelStorage.RemoveClueObjects())
                {
                    StaticPrefs.CoreGameHints--;
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameFlap);
                }
            }
            else
            {
                if (StaticPrefs.SelectedCoreGameLevel == 1)
                {
                    _levelStorage.RemoveClueObjects();
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameFlap);
                }
                else
                {
                    _hintModalUIPanel.Show();
                }
            }
        }

        public void AddDodoBird(Vector3 spawnPosition)
        {
            _dodoBirdCounter.UpdateValue(DodoBirdsService.Balance.ToString());
            _dodoBirdCounter.Show();
            _dodoBirdCounter.LaunchDodoBird(spawnPosition, OnCallback);
        }

        public void HideEndgameTimer()
        {
            _endgameTimerUIPanel.DOMove(_endgameTimerTarget.position, 1f).OnComplete(OnHideEndgameTimerComplete);
        }

        private void OnHideEndgameTimerComplete()
        {
            if(_isGameWin)
            {
                _endGameVictoryPanel.ShowIndicators();
            }
            else
            {
                _endGameDefeatPanel.ShowIndicators();
            }
        }

        private void OnCallback()
        {
            _dodoBirdsService.AddDodoBird();
            _dodoBirdCounter.BounceValue();
            _dodoBirdCounter.UpdateValue(DodoBirdsService.Balance.ToString());
            _dodoBirdCounter.Hide();
        }

        private void OnTimerUpdatedEvent(float value, float maxValue)
        {
            _topPanel.SetTime(Mathf.FloorToInt(value), Mathf.FloorToInt(maxValue));
        }

        private void OnTimerDeactivatedEvent()
        {
            _topPanel.DeactivateTimer();
        }
    }
}
