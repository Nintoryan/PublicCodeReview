using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame.UI.Panels
{
    public class ModalUIPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _parentBody;
        [SerializeField] private RectTransform _modalBody;
        
        [SerializeField] private Image _blackScreen;
        [SerializeField] protected Button _close;
        [Header("Animation")] 
        [SerializeField] private Vector2 _fromPos;
        [SerializeField] private float _fromScale;
        [SerializeField] private Vector2 _toPos;
        [SerializeField] private float _toScale;
        
        
        private const float Duration = 0.5f;
        
        private bool _canClose;

        private void Awake()
        {
            OnAwake();   
        }

        protected virtual void OnAwake()
        {
            _close.onClick.AddListener(()=>
            {
                Hide();
            });
        }



        [ContextMenu("TestShow")]
        private void TestShow()
        {
            _parentBody.SetActive(true);
            _blackScreen.color = new Color(_blackScreen.color.r, _blackScreen.color.g, _blackScreen.color.b, 0.56f);
            _modalBody.anchoredPosition = _toPos;
            _modalBody.localScale = Vector3.one * _toScale;
        }
        
        [ContextMenu("TestHide")]
        private void TestHide()
        {
            _parentBody.SetActive(false);
            _blackScreen.color = new Color(_blackScreen.color.r, _blackScreen.color.g, _blackScreen.color.b, 0f);
            _modalBody.anchoredPosition = _fromPos;
            _modalBody.localScale = Vector3.one * _fromScale;
        }

        public void Show()
        {
            _parentBody.SetActive(true);
            _blackScreen.color = new Color(_blackScreen.color.r, _blackScreen.color.g, _blackScreen.color.b, 0);
            _modalBody.anchoredPosition = _fromPos;
            _modalBody.localScale = Vector3.one * _fromScale;

            var s = DOTween.Sequence();
            s.Append(_blackScreen.DOFade(0.56f, Duration).SetEase(Ease.OutBack));
            s.Join(_modalBody.DOAnchorPos(_toPos, Duration).SetEase(Ease.OutBack));
            s.Join(_modalBody.DOScale(Vector3.one * _toScale, Duration).SetEase(Ease.OutBack));
            s.AppendCallback(() => 
            { 
                _canClose = true;
                OnShown();
            });
        }

        protected virtual void OnShown()
        {
            
        }

        public void Hide()
        {
            Hide(null);
        }
        public void Hide(Action onHidden = null)
        {
            _canClose = false;

            var s = DOTween.Sequence();
            s.Append(_blackScreen.DOFade(0, Duration).SetEase(Ease.InBack));
            s.Join(_modalBody.DOAnchorPos(_fromPos, Duration).SetEase(Ease.InBack));
            s.Join(_modalBody.DOScale(Vector3.one * _fromScale, Duration).SetEase(Ease.InBack));
            s.AppendCallback(() =>
            {
                _parentBody.SetActive(false);
                onHidden?.Invoke();
            });
        }

    }
}