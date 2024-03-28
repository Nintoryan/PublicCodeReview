using System;
using DG.Tweening;
using UnityEngine;
using Util;

namespace Tutorial
{
    public class TutorialFinger : MonoBehaviour
    {
        private Transform _originalParent; 
        [SerializeField] private Transform _body;
        [SerializeField] private float _bounceDuration = 1;
        [SerializeField] private float _showDuration = 1;
        [SerializeField] private float _hideDuration = 1;

        private Sequence _sequence;

        private Tween _bounceTween;
        private Tween _moveTween;
        private Tween _hideTween;
        private Tween _showTween;
        
        public enum AngleType
        {
            UpRight,
            DownRight,
            UpLeft,
            DownLeft
        }


        private void Awake()
        {
            _originalParent = transform.parent;
        }

        public void SetDefaultParent()
        {
            transform.SetParent(_originalParent);
            transform.localScale =Vector3.one;
        }
        public void ShowAndMoveBetweenWordsPoints(Vector3 point1, Vector3 point2, Vector3 offset, float duration = 1,
            AngleType angleType = AngleType.UpLeft)
        {
            ShowInPosition(point1, offset, angleType,
                OnShown: () => _moveTween = _body.DOMove(point2, duration).SetLoops(-1, LoopType.Yoyo));
        }


        public void PlayBounceInWordPoint(Vector3 point, Vector3 offset,
            AngleType angleType = AngleType.UpLeft)
        {
            ShowInPosition(point, offset, angleType, OnShown: () =>
                _bounceTween = _body.DOScale(Vector3.one * 0.7f, _bounceDuration).SetLoops(-1, LoopType.Yoyo));
        }

        public void StopAnimation(Action onHidden = null, bool withAnimation = true)
        {
            KillTween();
            if (withAnimation)
            {
                _hideTween = _body.DOScale(Vector3.zero, _hideDuration)
                    .OnComplete(() => onHidden?.Invoke())
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            }
            else
            {
                _body.localScale = Vector3.zero;
                onHidden?.Invoke();
            }
        }

        private void ShowInPosition(Vector3 point, Vector3 offset,
            AngleType angleType = AngleType.UpLeft,
            Action OnShown = null)
        {
            KillTween();
            _body.localScale = Vector3.zero;
            SetRotation(angleType);
            _body.position = point + offset;
           _showTween =  _body.DOScale(Vector3.one, _showDuration).OnComplete(() => OnShown?.Invoke());
        }

        private void SetRotation(AngleType angleType)
        {
            switch (angleType)
            {
                case AngleType.UpRight:
                    _body.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case AngleType.DownRight:
                    _body.rotation = Quaternion.Euler(0, 0, -180);
                    break;
                case AngleType.UpLeft:
                default:
                    _body.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case AngleType.DownLeft:
                    _body.rotation = Quaternion.Euler(0, 0, 90);
                    break;
            }
        }

        private void KillTween()
        {
            _moveTween?.Kill();
            _bounceTween?.Kill();
            _hideTween?.Kill();
            _showTween?.Kill();
        }
    }
}