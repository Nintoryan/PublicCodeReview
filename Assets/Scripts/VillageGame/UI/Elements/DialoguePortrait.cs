using System;
using DG.Tweening;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace VillageGame.UI.Elements
{
    public class DialoguePortrait : MonoBehaviour
    {
        private const float ANIMATION_DURATION = 0.7f;
        [SerializeField] private RectTransform _body;
        [SerializeField] private Image _bodyImage;
        [SerializeField] private Image _emotionImage;
        [SerializeField] private bool _isLeft;
        public bool IsLeft => _isLeft;
        public CharacterType CurrentCharacter { get; private set; }

        private Tween _tween;

        public void Show()
        {
            _tween?.Kill();
            _tween = _body.DOLocalMoveX(0, ANIMATION_DURATION)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public void Hide(bool withAnimation = true, Action OnHidden = null)
        {
            _tween?.Kill();
            float distance = -600;
            if (withAnimation)
            {
                _tween = _body.DOLocalMoveX(distance, ANIMATION_DURATION)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => { OnHidden?.Invoke(); })
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            }
            else
            {
                _body.anchoredPosition = new Vector3(distance, _body.localPosition.y, 0);
            }

            CurrentCharacter = CharacterType.None;
        }

        public void SetCharacterType(CharacterType type)
        { 
            CurrentCharacter = type;
        }

        public void SetPortrait(Sprite portrait, Sprite emotion)
        {
            _bodyImage.sprite = portrait;
            if (emotion == null)
            {
                _emotionImage.color = new Color32(255, 255, 255, 0);
            }
            else
            {
                _emotionImage.color = new Color32(255, 255, 255, 255);
                _emotionImage.sprite = emotion;
            }
        }
    }
}