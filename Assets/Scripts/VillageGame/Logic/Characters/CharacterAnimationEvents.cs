using System;
using Data.Scripts.Audio;
using UnityEngine;

namespace VillageGame.Logic.Characters
{
    public class CharacterAnimationEvents: MonoBehaviour
    {

        [SerializeField] private CharacterAnimation _characterAnimation;
        public event Action EndPineConeAnimationEvent;
        private void InvokeEndPineConeAnimation()
        {
            _characterAnimation.EndCutScene();
            EndPineConeAnimationEvent?.Invoke();
        }

        private void PlayPineConeAudio()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.PineCone);
        }
        private void EndCutSceneAnimation()
        {
            _characterAnimation.EndCutScene();
        }
    }
}