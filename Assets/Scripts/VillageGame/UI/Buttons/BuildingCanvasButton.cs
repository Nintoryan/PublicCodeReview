using System;
using DG.Tweening;
using UnityEngine;
using Util;

namespace VillageGame.UI.Buttons
{
    public class BuildingCanvasButton: UIButton
    {
        [SerializeField] private float _animationDuration = 0.7f;
        
        public Action ClickBuildEvent;
        
        public override void Show(Action onShown = null)
        {
            body.DOScale(Vector3.one, _animationDuration)
                .SetEase(Ease.OutBack)
                .OnComplete( () => onShown?.Invoke())
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
        public override void Hide(Action onHidden = null)
        {
            body.DOScale(Vector3.zero, _animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => onHidden?.Invoke())
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
        
        public void SetHidden()
        {
            body.localScale = Vector3.zero;
        }
        
        protected override void OnClick()
        {
            ClickBuildEvent?.Invoke();
        }
    }
}