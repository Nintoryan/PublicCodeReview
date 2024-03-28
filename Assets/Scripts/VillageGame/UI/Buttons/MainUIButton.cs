using System;
using DG.Tweening;
using UnityEngine;
using Util;

namespace VillageGame.UI.Buttons
{
    public class MainUIButton : UIButton
    {
        private const float ANIMATION_DURATION = 0.35f;
        [SerializeField] private RectTransform _notificationBody;
        public Action ClickEvent;
        private bool _isShow = true;

        public Action UnblockClickEvent;


        private bool _isBlock;

        public void HideNoAnimation()
        {
            body.localScale = Vector3.zero;
            _isShow = false;
        }

        public override void Show(Action onShown = null)
        {
            body.DOScale(Vector3.one, ANIMATION_DURATION)
                .SetEase(Ease.OutBack)
                .OnComplete(() => onShown?.Invoke())
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        public override void Hide(Action onHidden = null)
        {
            _isShow = false;
            body.DOScale(Vector3.zero, ANIMATION_DURATION)
                .SetEase(Ease.InBack)
                .OnComplete(() => onHidden?.Invoke())
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        public void ShowNotification(bool isAnimation)
        {
            if (isAnimation)
            {
                _notificationBody.DOScale(Vector3.one, ANIMATION_DURATION)
                    .SetEase(Ease.OutBack)
                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
            else
            {
                _notificationBody.localScale = Vector3.one;
            }
        }

        public void HideNotification(bool isAnimation)
        {
            if (isAnimation)
            {
                _notificationBody.DOScale(Vector3.zero, ANIMATION_DURATION)
                    .SetEase(Ease.InBack)
                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
            else
            {
                _notificationBody.localScale = Vector3.zero;
            }
        }

        public override void DisableButton()
        {
            _isShow = false;
            gameObject.SetActive(false);
        }

        public override void EnableButton()
        {
            body.localScale = Vector3.zero;
            gameObject.SetActive(true);
        }

        public void BlockDefaultClickEvent(bool isBlock)
        {
            _isBlock = isBlock;
        }
        
        protected override void OnClick()
        {
            UnblockClickEvent?.Invoke();
            if (_isBlock)
            {
                return;
            }
            ClickEvent?.Invoke();
        }
    }
}