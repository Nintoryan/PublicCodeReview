using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VillageGame.UI.Elements
{
    public class TextPanel : MonoBehaviour
    {
        private const float ANIMATION_DURATION = 0.45f;
        [SerializeField] private RectTransform _body;
        [SerializeField] private TextMeshProUGUI _text;

        public void SetText(string text)
        {
            _text.SetText(text);
        }

        public void SetTextColor(Color32 color)
        {
            _text.color = color;
        }

        public void ShowAfterHide(Action onHidden = null)
        {
            var sequence = DOTween.Sequence();

            sequence.Append(_body.DOScale(Vector3.zero, ANIMATION_DURATION)
                .OnComplete( () => onHidden?.Invoke())
                .SetEase(Ease.InBack));
            sequence.Append(_body.DOScale(Vector3.one, ANIMATION_DURATION).SetEase(Ease.OutBack));
        }
        public void Show(bool withAnimation)
        {
            if (withAnimation)
            {
                _body.DOScale(Vector3.one, ANIMATION_DURATION)
                    .SetEase(Ease.OutBack)
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            }
            else
            {
                _body.localScale = Vector3.one;
            }
        }

        public void Hide(bool withAnimation, Action OnHidden = null)
        {
            if (withAnimation)
            {
                _body.DOScale(Vector3.zero, ANIMATION_DURATION)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => OnHidden?.Invoke())
                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
            else
            {
                _body.localScale = Vector3.zero;
            }
        }

        public void EnablePanel()
        {
            _body.gameObject.SetActive(true);
        }

        public void DisablePanel()
        {
            _body.gameObject.SetActive(false);
        }
    }
}