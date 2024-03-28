using System;
using Data.Scripts.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Data.Types;

namespace VillageGame.UI.Panels
{
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField] protected RectTransform body;
        [SerializeField] private bool _isBlockInput;
        [SerializeField] private bool _isWindow;
        [SerializeField] private bool _animated;
        [SerializeField] private bool _isPlaySound = true;
        [SerializeField] protected float _upY;

        [SerializeField] private WindowType _windowType;
        [SerializeField] private GameObject _closeButtonObject;
        
        public bool IsActive => body.gameObject.activeInHierarchy;
        public bool IsBlockInput => _isBlockInput;
        public bool IsWindow => _isWindow;
        public WindowType Type => _windowType;

        public event Action<UIPanel, bool> PanelSwitchEvent;
        
        protected void InvokeSwitchEvent(bool isOpen)
        {
            PanelSwitchEvent?.Invoke(this,isOpen);
        }
        public virtual void Show(Action onShown = null)
        {
            if (_isPlaySound)
            {
                AudioManager.Instance.PlayAudioEvent(AudioEventType.MenuOpen);
            }
            if (!body.gameObject.activeInHierarchy)
            {
                body.gameObject.SetActive(true);
            }

            if (_animated && body.anchoredPosition.y != _upY)
            {
                body.anchoredPosition = new Vector2(0, -3000);
                body.DOAnchorPosY(_upY, 0.5f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        onShown?.Invoke();
                        OnShown();
                        PanelSwitchEvent?.Invoke(this, true);
                    });
            }
            else
            {
                onShown?.Invoke();
                OnShown();
                PanelSwitchEvent?.Invoke(this, true);
            }

            Debugging.Log($"UIPanel: show {gameObject.name}", ColorType.Red);
        }

        public virtual void ShowNoAction()
        {
            Show();
        }

        public virtual void Hide(Action onHidden = null)
        {
            if (_isPlaySound)
            {
                AudioManager.Instance.PlayAudioEvent(AudioEventType.MenuOpen);
            }
            if (_animated)
            {
                var s = DOTween.Sequence();
                s.Append(body.DOAnchorPosY(-3000, 0.5f).SetEase(Ease.InBack)
                    .SetLink(gameObject, LinkBehaviour.CompleteOnDisable));
                s.AppendCallback(() =>
                {
                    onHidden?.Invoke();
                    PanelSwitchEvent?.Invoke(this, false);
                    body.gameObject.SetActive(false);
                });
            }
            else
            {
                onHidden?.Invoke();
                PanelSwitchEvent?.Invoke(this, false);
                body.gameObject.SetActive(false);
            }

            Debugging.Log($"UIPanel: hide {gameObject.name}", ColorType.Red);
        }

        public virtual void HideNoAnimation()
        {
            if (!IsActive) return;

            body.gameObject.SetActive(false);
            PanelSwitchEvent?.Invoke(this, false);
        }

        public virtual void HideNoAction()
        {
            Hide();
        }

        protected virtual void OnShown()
        {

        }

        public virtual void Switch()
        {
            if (IsActive)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void DisableCloseButton()
        {
            _closeButtonObject?.SetActive(false);
        }
        public void EnableCloseButton()
        {
            _closeButtonObject?.SetActive(true);
        }
    }
}