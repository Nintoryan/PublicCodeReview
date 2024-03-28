using Data.Scripts.Audio;
using System;
using TMPro;
using UnityEngine;
using VillageGame.UI.Panels;

namespace VillageGame.Logic.Curtains
{
    public class CloudCurtain : UIPanel
    {
        [Space, Header("Cload curtain components")] 
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Animator _animator;
        
        private static readonly int _show = Animator.StringToHash("Show");
        private static readonly int _showWithText = Animator.StringToHash("ShowWithText");

        private event Action OnShown;
        private event Action OnHidden;


        public static event Action EndAnimationEvent;
        public static event Action StartAnimationEvent;
        public static event Action OnShownEvent;
        public static event Action OnHiddenEvent;
        public static bool IsAnimating { get; private set; }

 
        public void Show(Action onShown = null, Action onHidden = null)
        {
            StartAnimationEvent?.Invoke();
            OnShown += onShown;
            OnHidden += onHidden;
            if (IsAnimating)
            {
                return;
            }
            IsAnimating = true;
            _animator.SetTrigger(_show);
            InvokeSwitchEvent(true);
        }

        public void ShowWithText(string text, Action onShown = null, Action onHidden = null)
        {
            StartAnimationEvent?.Invoke();
            OnShown += onShown;
            OnHidden += onHidden;
            if (IsAnimating)
            {
                return;
            }
            IsAnimating = true;
            _text.SetText(text);
            _animator.SetTrigger(_showWithText);
            InvokeSwitchEvent(true);
        }
        
        #region Animation Event

        private void InvokeOnShown()
        {
            OnShownEvent?.Invoke();
            OnShown?.Invoke();
            OnShown = null;
        }

        private void InvokeOnHidden()
        {
            OnHiddenEvent?.Invoke();
            IsAnimating = false;
            OnHidden?.Invoke();
            OnHidden = null;
            EndAnimationEvent?.Invoke();
            InvokeSwitchEvent(false);
        }

        private void PlayCloudOpenAudioEvent()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.CloudsOpen);
        }

        private void PlayCloudCloseAudioEvent()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.CloudsClose);
        }
        #endregion
    }
}
