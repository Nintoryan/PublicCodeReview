using System;
using Data.Scripts.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VillageGame.UI.Buttons
{
    public class TapPresentButton : UIButton
    {
        [SerializeField] private Image _presentClosed;
        [SerializeField] private Image _presentOpened;

        [SerializeField] private float _increasingDuration = 0.075f;
        [SerializeField] private float _deltaInc = 2.5f;
        [SerializeField] private float _deltaDec = -35f;

        private const float _sizeMin = 1;
        private const float _sizeMax = 1.8f;
        private float _sizeCurrent;

        private float _increasingTimer;

        public event Action ClickingDone;

        private bool _loadingDone;
        private bool _isOpen;

        private int _clickCount;

        protected override void OnClick()
        {
            if (!_isOpen)
            {
                _increasingTimer = _increasingDuration;
                AudioManager.Instance.PlayAudioClip(
                    AudioEventType.PresentClick,
                    clipIndex: MapSizeToValue(_sizeCurrent, _sizeMin, _sizeMax, 0, 5));
            }
        }

        private void Update()
        {
            if (_isOpen)
            {
                return;
            }

            if (_increasingTimer > 0)
            {
                _increasingTimer -= Time.deltaTime;
            }

            var delta = _increasingTimer > 0 ? _deltaInc : _deltaDec;
            _sizeCurrent = Mathf.Clamp(_sizeCurrent + delta * Time.deltaTime, _sizeMin, _sizeMax);
            transform.localScale = Vector3.one * _sizeCurrent;

            if (_sizeCurrent >= _sizeMax * 0.95f)
            {
                _isOpen = true;

                Opened();
            }
        }

        private void Opened()
        {
            var s = DOTween.Sequence();
            s.Append(transform.DOScale(Vector3.one * (_sizeMax * 1.25f), 0.75f));
            s.Append(_presentClosed.DOFade(0, 0.25f));
            s.AppendCallback(() => AudioManager.Instance.PlayAudioEvent(AudioEventType.PresentOpen));
            s.Join(_presentOpened.DOFade(1, 0.25f));
            s.AppendInterval(0.5f);
            s.AppendCallback(() => { ClickingDone?.Invoke(); });
        }

        public void Reset()
        {
            _presentClosed.color = new Color(_presentClosed.color.r, _presentClosed.color.g, _presentClosed.color.b, 1);
            _presentOpened.color = new Color(_presentOpened.color.r, _presentOpened.color.g, _presentOpened.color.b, 0);
            _sizeCurrent = _sizeMin;
            _increasingTimer = 0;
            _isOpen = false;
        }

        private int MapSizeToValue(float sizeCurrent, float sizeMin, float sizeMax, int valueMin, int valueMax)
        {
            // Преобразование линейной интерполяции
            float normalizedValue = Mathf.InverseLerp(sizeMin, sizeMax, sizeCurrent);
            float mappedValue = Mathf.Lerp(valueMin, valueMax, normalizedValue);

            // Округление до ближайшего целого числа
            int roundedValue = Mathf.RoundToInt(mappedValue);

            return roundedValue;
        }
    }
}