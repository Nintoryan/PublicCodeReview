using System.Linq;
using Data.Scripts.Audio;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "SO/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        public AudioEventData[] AudioEvents;

        public AudioEventData GetAudioEvent(AudioEventType type)
        {
            return AudioEvents.FirstOrDefault(a => a.Type == type);
        }
    }
}