using System;
using System.Collections;
using UnityEngine;
using Web.Api;

namespace Data.Scripts.Audio
{
    [Serializable]
    public class MainAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _villageMusicSource;
        [SerializeField] private AudioSource _coreGameMusicSource;
        [SerializeField] private float _blendTime = 5;
        [SerializeField] private float _maxVolume = 0.7f;

        [SerializeField] private AudioListener _listener;
        
        private AudioSource _currentSource;

        private void Awake()
        {
            _currentSource = _villageMusicSource;
            _currentSource.volume = _maxVolume;
            _currentSource.Play();
        }
        

        private void Update()
        {
            _listener.enabled = Time.deltaTime < 0.2f;
            AudioListener.volume = Time.deltaTime < 0.2f ? 1 : 0;
            //Debug.Log($"_listener:{_listener.enabled}, AudioListener.volume:{AudioListener.volume} deltaTime:{Time.deltaTime}");
        }

        public void PlayVillageTrack()
        {
            if (_currentSource == _villageMusicSource)
            {
                return;
            }
            StopAllCoroutines();
            BlendSounds(_villageMusicSource);
        }

        public void PlayCoreGameTrack()
        {
            if (_currentSource == _coreGameMusicSource)
            {
                return;
            }
            StopAllCoroutines();
            BlendSounds(_coreGameMusicSource);
        }

        private void BlendSounds(AudioSource busy)
        {
            StartCoroutine(FadeOut(_currentSource));
            StartCoroutine(FadeIn(busy));
        }

        private IEnumerator FadeIn(AudioSource audioSource)
        {
            audioSource.volume = .01f;
            audioSource.Play();

            while (audioSource.volume < _maxVolume)
            {
                audioSource.volume += Time.deltaTime / _blendTime;

                yield return new WaitForEndOfFrame();
            }

            audioSource.volume = _maxVolume;
            _currentSource = audioSource;
        }

        private IEnumerator FadeOut(AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {
                float startVolume = audioSource.volume;

                while (audioSource.volume > 0)
                {
                    audioSource.volume -= Time.deltaTime / _blendTime * 1.2f;

                    yield return new WaitForEndOfFrame();
                }

                audioSource.Stop();
                audioSource.volume = 0;
            }
        }
    }
}