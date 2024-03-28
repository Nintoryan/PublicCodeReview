using UnityEngine;

namespace Data.Scripts.Audio
{
    public class AnimationAudioEvents: MonoBehaviour
    {
        private void PlayShowStartEvent()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.ShowStar);
        }
    }
}