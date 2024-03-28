using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class ImageColorToggle : MonoBehaviour
    {
        [SerializeField] protected Image _editableImage;
        [SerializeField] private Color _disabledColor = new(1,1,1,1);
        [SerializeField] private Color _enabledColor= new(1,1,1,1);

        public void PlayEnableColorAnimation(float duration = 1)
        {
            _editableImage.DOColor(_enabledColor,duration)
                .SetLoops(2, LoopType.Yoyo)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
        public void PlayDisableColorAnimation()
        {
            _editableImage.DOColor(_disabledColor,1)
                .SetLoops(2, LoopType.Yoyo)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
        
        public void SetDisableColor()
        {
            if (_editableImage == null)
            {
                return;
            }

            _editableImage.color = _disabledColor;
        }

        public void SetEnableColor()
        {
            if (_editableImage == null)
            {
                return;
            }
            
            _editableImage.color = _enabledColor;
        }

        private void OnValidate()
        {
            if (_editableImage == null)
            {
                _editableImage = GetComponent<Image>();
            }
        }

      
    }
}
