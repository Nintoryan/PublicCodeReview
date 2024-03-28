using System;
using System.Collections;
using Data.Scripts.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VillageGame.UI.Buttons
{
    public abstract class UIButton : MonoBehaviour
    {
        protected readonly float ANIMATION_DURATION = 0.25f;

        public RectTransform body;
        [SerializeField] protected Button button;
        [SerializeField] private bool _isImpactAnimation = true;

        protected bool _isCooldown;
        protected Coroutine _cooldownCoroutine;

        protected virtual void Start()
        {
            if(button == null) return;

           // button.onClick.RemoveAllListeners();
            button.onClick.AddListener(Click);
            if (_isImpactAnimation)
            {
                button.onClick.AddListener(PlayImpact);
            }
        }


        public virtual void Show(Action onShown = null)
        {
            gameObject.SetActive(true);
            onShown?.Invoke();
        }

        public virtual void Hide(Action onHidden = null)
        {
            gameObject.SetActive(false);
            onHidden?.Invoke();
        }

        public virtual void EnableButton()
        {
            if(button == null) return;
            button.interactable = true;
        }

        public virtual void DisableButton()
        {
            if(button == null) return;
            button.interactable = false;
        }

        protected abstract void OnClick();

        public void Click()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.PressButton);
            OnClick();
        }

        public virtual void PlayImpact()
        {
            if(button == null) return;
            if (_isCooldown || !gameObject.activeInHierarchy)
            {
                return;
            }

            body.DOScale(body.localScale * 0.9f, ANIMATION_DURATION).SetEase(Ease.InCirc).SetLoops(2, LoopType.Yoyo);
            _cooldownCoroutine = StartCoroutine(StartCooldown(ANIMATION_DURATION * 2));
        }


        protected IEnumerator StartCooldown(float time)
        {
            _isCooldown = true;
            yield return new WaitForSeconds(time);
            _isCooldown = false;
        }

        private void OnDisable()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
            }

            _isCooldown = false;
        }

        private void OnValidate()
        {
            if (button == null)
            {
                if (!TryGetComponent(out button))
                {
                    button = GetComponentInChildren<Button>();
                }
            }

            if (body == null)
            {
                TryGetComponent(out body);
            }
        }
    }
}