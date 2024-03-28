using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VillageGame.UI.Buttons
{
    public class EnterNameOKButton : EventButton
    {
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Color32 _enableColor = new Color32(255, 255, 255, 255);
        [SerializeField] private Color32 _disableColor = new Color32(255, 255, 255, 255);

        public void SetEnableColor()
        {
            _buttonImage.color = _enableColor;
        }

        public void SetDisableColor()
        {
            _buttonImage.color = _disableColor;
        }
        
        public void PlayFailureImpact()
        {
            if (_isCooldown || !gameObject.activeInHierarchy)
            {
                return;
            }
            body.DOShakeRotation(duration: ANIMATION_DURATION * 2, strength: 10);
            _cooldownCoroutine = StartCoroutine(StartCooldown(ANIMATION_DURATION * 2));
        }
    }
}