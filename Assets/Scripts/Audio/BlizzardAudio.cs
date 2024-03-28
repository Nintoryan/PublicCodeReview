using System.Collections;
using UnityEngine;

namespace Data.Scripts.Audio
{
    public class BlizzardAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _blizzardAudioSource;
        [SerializeField] private float _blendTime = 3;
        private Coroutine _coroutine;

        private void Awake()
        {
            _blizzardAudioSource.volume = 0;
            _blizzardAudioSource.Stop();
        }

        public void StartPlay()
        {
            _blizzardAudioSource.enabled = true;
            _coroutine = StartCoroutine(Start());
        }

        public void StopPlay()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

;
            _coroutine = StartCoroutine(Stop());
        }
        private IEnumerator Start()
        {
            _blizzardAudioSource.volume = .01f;
            _blizzardAudioSource.Play();

            while (_blizzardAudioSource.volume < 1)
            {
                _blizzardAudioSource.volume += Time.deltaTime / _blendTime;

                yield return new WaitForEndOfFrame();
            }

            _blizzardAudioSource.volume = 1;
        }

        private IEnumerator Stop()
        {
            if (_blizzardAudioSource.isPlaying)
            {
                float startVolume = _blizzardAudioSource.volume;

                while (_blizzardAudioSource.volume > 0)
                {
                    _blizzardAudioSource.volume -= Time.deltaTime / _blendTime * 1.2f;

                    yield return new WaitForEndOfFrame();
                }

                _blizzardAudioSource.volume = 0;
                _blizzardAudioSource.Stop();
                _blizzardAudioSource.enabled = false;
            }
        }
    }
}