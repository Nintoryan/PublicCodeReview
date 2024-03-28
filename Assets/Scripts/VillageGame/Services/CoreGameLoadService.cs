using System;
using System.Collections.Generic;
using SO;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Panels;
using CoreGame.SO;
using Data.Scripts.Audio;
using DefaultNamespace.VillageGame.Infrastructure;
using VillageGame.UI.Panels.Warnings;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;

namespace VillageGame.Services
{
    public class CoreGameLoadService : MonoBehaviour, ILoading
    {
        [SerializeField] private List<GameObject> _objectsToDisable;

        private PlayerPrefsConfig _playerPrefsConfig;
        private InputService _inputService;
        private SceneLoaderService _sceneLoaderService;
        private EnergyStorage _energyStorage;
        private EnergyWarningUIPanel _energyWarningPanel;
        private CurrencyStorage _currencyStorage;
        private LevelDatabase _levelDatabase;

        private int _lastStartedCoreGameLevel = -1;

        public Action<int> CoreGameWinEvent;
        public Action<int> CoreGameLoseEvent;
        public Action DodoCombinationCompletedEvent;

        public Action LoadCoreGameSceneEvent;
        public Action UnLoadCoreGameSceneEvent;
        public Action<int> LastStartedCoreGameLevelUpdatedEvent;

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _playerPrefsConfig = objectResolver.Resolve<PlayerPrefsConfig>();
            _sceneLoaderService = objectResolver.Resolve<SceneLoaderService>();
            _inputService = objectResolver.Resolve<InputService>();
            _energyStorage = objectResolver.Resolve<EnergyStorage>();
            _energyWarningPanel = objectResolver.Resolve<UIFacade>().FindWarningPanel<EnergyWarningUIPanel>();
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            _levelDatabase = objectResolver.Resolve<LevelDatabase>();
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
            PlayerPrefs.DeleteAll();
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _sceneLoaderService.SceneUnloadedEvent += OnSceneUnloaded;
            }
            else
            {
                _sceneLoaderService.SceneUnloadedEvent -= OnSceneUnloaded;
            }
        }

        public void LoadCoreGameScene()
        {
            if (_energyStorage.IsEnoughEnergy(1))
            {
                _energyStorage.Remove(1);

                foreach (GameObject obj in _objectsToDisable)
                {
                    obj.SetActive(false);
                }

                _inputService.BlockInput(true);
                PlayerPrefs.SetInt(_playerPrefsConfig.PlayingLevelFlag, 1);

                if (StaticPrefs.SelectedCoreGameLevel >= 0)
                {
                    if (StaticPrefs.SelectedCoreGameLevel >= _levelDatabase.Levels.Count)
                    {
                        if (_lastStartedCoreGameLevel == -1)
                        {
                            int selectedLevel = -1;
                            int safeCounter = 100;
                            do
                            {
                                safeCounter--;
                                List<int> possibleLevelIds = new();
                                for (int i = _levelDatabase.CyclicLevelStart; i < _levelDatabase.Levels.Count; i++)
                                {
                                    RandomizedLevelData v = _levelDatabase.Levels[i];
                                    if (v.Tutorial == null)
                                    {
                                        possibleLevelIds.Add(i);
                                    }
                                }
                                int randomId = UnityEngine.Random.Range(0, possibleLevelIds.Count - 1);
                                selectedLevel = possibleLevelIds[randomId];
                            }
                            while (safeCounter > 0 && selectedLevel == StaticPrefs.LastFinishedCoreGameLevel);

                            StaticPrefs.LastStartedCoreGameLevel = selectedLevel;
                            _lastStartedCoreGameLevel = selectedLevel;
                        }
                        else
                        {
                            StaticPrefs.LastStartedCoreGameLevel = _lastStartedCoreGameLevel;
                        }
                    }
                    else
                    {
                        StaticPrefs.LastStartedCoreGameLevel = StaticPrefs.SelectedCoreGameLevel;
                    }
                }
                else
                {
                    StaticPrefs.EnergyValue = 8;
                    StaticPrefs.CoreGameShuffles = 10;
                    StaticPrefs.CoreGameHints = 10;
                }

                GlobalEvent.LoadCoreGameSceneInvoke();
                AudioManager.Instance.MainAudio.PlayCoreGameTrack();
                LastStartedCoreGameLevelUpdatedEvent?.Invoke(StaticPrefs.LastStartedCoreGameLevel);
                LoadCoreGameSceneEvent?.Invoke();

                _sceneLoaderService.LoadSceneAdditive(id: 1);
            }
            else
            {
                _energyWarningPanel.Show();
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (scene.buildIndex == 1)
            {
                if (PlayerPrefs.GetInt(_playerPrefsConfig.ReloadFlag) == 0)
                {
                    GlobalEvent.LoadVillageSceneInvoke();
                    foreach (GameObject obj in _objectsToDisable)
                    {
                        obj.SetActive(true);
                    }
                    _inputService.BlockInput(false);

                    CheckGameResult();
                    UnLoadCoreGameSceneEvent?.Invoke();
                    AudioManager.Instance.MainAudio.PlayVillageTrack();
                }
                else
                {
                    CheckGameResult();
                    LoadCoreGameScene();
                }
            }
        }

        private void CheckGameResult()
        {
            if (PlayerPrefs.GetInt(_playerPrefsConfig.VictoryFlag) == 1)
            {
                StaticPrefs.SelectedCoreGameLevel++;
                CoreGameWinEvent?.Invoke(StaticPrefs.SelectedCoreGameLevel);
                if (StaticPrefs.IsDodoCombinationCompleted)
                {
                    DodoCombinationCompletedEvent?.Invoke();
                    StaticPrefs.IsDodoCombinationCompleted = false;
                }
                StaticPrefs.LastFinishedCoreGameLevel = _lastStartedCoreGameLevel;
                _lastStartedCoreGameLevel = -1;
                LastStartedCoreGameLevelUpdatedEvent?.Invoke(-1);
            }
            else
            {
                CoreGameLoseEvent?.Invoke(StaticPrefs.SelectedCoreGameLevel);
            }
        }

        public void Load(LoadData request)
        {
            if (request.data.last_started_core_game_level >= 0)
            {
                _lastStartedCoreGameLevel = request.data.last_started_core_game_level;
            }
            else
            {
                _lastStartedCoreGameLevel = -1;
            }
        }
    }
}
