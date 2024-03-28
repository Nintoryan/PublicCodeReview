using SO;
using UnityEngine;

namespace Data.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private MainAudio _mainAudio;
        [Space]
        [SerializeField] private AudioSource _eventAudioSource;
        public MainAudio MainAudio => _mainAudio;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlayAudioEvent(AudioEventType type)
        {
            var audioEvent = _audioConfig.GetAudioEvent(type);
            if (audioEvent != null)
            {
                audioEvent.Play(_eventAudioSource);
            }
        }

        public void PlayAudioClip(AudioEventType type, int clipIndex)
        {
            var audioEvent = _audioConfig.GetAudioEvent(type);
            if (audioEvent != null)
            {
                audioEvent.Play(_eventAudioSource, clipIndex);
            }
        }
    }
}