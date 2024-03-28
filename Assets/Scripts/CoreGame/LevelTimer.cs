using System;
using System.Collections;
using Data.Scripts.Audio;
using SO;
using UnityEngine;

namespace CoreGame
{
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] private LevelStorage _levelStorage;

        public Action TimerStartedEvent;
        public Action TimerEndedEvent;
        public Action<float,float> TimerUdpatedEvent;
        public Action TimerDeactivatedEvent;
        public static Action<bool, int> CoreGameDefeatedEvent;

        private float _time;
        private float _maxTime;
        private Coroutine _timerCoroutine;

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
                _levelStorage.GameWonEvent += OnGameWonEvent;
            }
            else
            {
                _levelStorage.GameWonEvent -= OnGameWonEvent;
            }
        }

        private void Start()
        {
            _maxTime = _levelStorage.GetLevelTime();
            _time = _maxTime;
            if (_maxTime > 0)
            {
                _timerCoroutine = StartCoroutine(StartTimer());
            }
            else
            {
                TimerDeactivatedEvent?.Invoke();
            }
        }

        public void AddTime(float time)
        {
            _time += time;
            _timerCoroutine = StartCoroutine(StartTimer());
        }

        private void OnGameWonEvent(int levelReward, int selectedLevel, bool isNextLevelButtonActive)
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
            }
        }

        private IEnumerator StartTimer()
        {
            TimerStartedEvent?.Invoke();
            while (_time > 0f)
            {
                TimerUdpatedEvent?.Invoke(_time,_maxTime);
                _time -= Time.deltaTime;
                yield return null;
            }
            CoreGameDefeatedEvent?.Invoke(false, StaticPrefs.LastStartedCoreGameLevel);
            TimerEndedEvent?.Invoke();
            AudioManager.Instance?.PlayAudioEvent(AudioEventType.CoreGameTimeOut);
        }
    }
}
