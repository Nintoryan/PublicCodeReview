using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VillageGame.UI.Indicators
{
    public abstract class UIIndicator : MonoBehaviour
    {
        [SerializeField] protected RectTransform body;
        [SerializeField] protected TextMeshProUGUI valueText;

        private bool _isShow = true;
        
        public virtual void UpdateValue(string value) 
        { 
            valueText.text = value;
        }

        public void Hide()
        {
            if (!_isShow)
            {
                return;
            }

            _isShow = false;
            
            body.DOScale(Vector3.zero, 0.35f)
                .SetEase(Ease.InBack)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        public void Show()
        {
            if (_isShow)
            {
                return;
            }
            
            _isShow = true;

            body.DOScale(Vector3.one, 0.35f)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
    }
}
