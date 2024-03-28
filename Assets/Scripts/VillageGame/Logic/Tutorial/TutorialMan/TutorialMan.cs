using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VillageGame.Logic.Tutorial
{
    public abstract class TutorialMan : MonoBehaviour
    {
        private const float ANIMATION_DURATION = 0.4f;
        private const float DISABLE_DISTANCE = 2000;
        
        [SerializeField] protected RectTransform _body;
        private Tween _tween;

        private bool _isShown;
        private void Awake()
        {
            Hide(withAnimation: false);
        }
        
        public virtual void Show(Action OnShown = null, bool withAnimation = true)
        {
            if (_isShown)
            {
                return;
            }

            _isShown = true;
            _tween?.Kill();
            EnableTutor();
            
            if (withAnimation)
            {
                _body.anchoredPosition = new Vector2(DISABLE_DISTANCE, _body.anchoredPosition.y);
                _tween = _body.DOAnchorPosX(0, ANIMATION_DURATION)
                    .OnComplete(() => OnShown?.Invoke())
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            }
            else
            {
                OnShown?.Invoke();
                _body.anchoredPosition = Vector2.zero;
            }
        }

        public virtual void Hide(Action OnHidden = null, bool withAnimation = true)
        {
            _isShown = false;
            _tween?.Kill();

            if (withAnimation)
            {
                _tween = _body.DOAnchorPosX(DISABLE_DISTANCE, ANIMATION_DURATION)
                    .OnComplete(() => OnHidden?.Invoke())
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            }
            else
            {
                _body.anchoredPosition = new Vector2(DISABLE_DISTANCE, 0);
                DisableTutor();
                OnHidden?.Invoke();
            }
            
        }

        protected virtual void DisableTutor()
        {
            _body.gameObject.SetActive(false);
        }
        
        protected virtual void EnableTutor()
        {
            _body.gameObject.SetActive(true);
        }
    }
}