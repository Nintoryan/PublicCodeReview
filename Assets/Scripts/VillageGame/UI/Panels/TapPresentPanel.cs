using System;
using DG.Tweening;
using SO;
using SO.Data.Presents;
using UnityEngine;
using UnityEngine.UI;
using VillageGame.Services;
using VillageGame.UI.Buttons;
using Web.Api;

namespace VillageGame.UI.Panels
{
    public class TapPresentPanel : UIPanel
    {
        [SerializeField] private TapPresentButton _presentButton;
        [SerializeField] private Sprite _closed;
        [SerializeField] private Sprite _opened;

        [SerializeField] private Image _presentClosed;
        [SerializeField] private Image _presentOpened;

        [SerializeField] private GameObject _error;

        public event Action Error;
        public event Action Success;

        private Action _success;
        private bool _loaded;
        public TapPresentButton PresentButton => _presentButton;

        private void Start()
        {
            _presentButton.ClickingDone += OnClickingDone;
        }

        private void OnClickingDone()
        {
            if (_loaded)
            {
                Success?.Invoke();
                _success?.Invoke();
            }
            else
            {
                ShowError();
            }

            _success = null;
        }

        public void Initialize(CalendarPresentButton button, Action success)
        {
            _loaded = false;
            _error.gameObject.SetActive(false);
            _presentButton.gameObject.SetActive(true);
            _presentButton.Reset();
            SetPresentSprites(button.Data.BoxSprite, button.Data.OpenBoxSprite);
            _success = success;

            if (button.Data.PresentValue is PromocodePresentValueData promo)
            {
                var key = $"{JSAPI.UID}-{promo.PromocodeID}-{promo.Description.GetHashCode()}";
                PromoCodeService.GetPromoCode(promo.PromocodeID, key, PromoCodeSuccess, null);
            }
            else
            {
                _loaded = true;
            }
        }

        public void Initialize(DodoBirdGift dodoGift, Action success)
        {
            _loaded = true;
            _error.gameObject.SetActive(false);
            _presentButton.gameObject.SetActive(true);
            _presentButton.Reset();
            SetPresentSprites(dodoGift.Closed, dodoGift.Opened);
            _success = success;
        }

        private void PromoCodeSuccess(string promoCode)
        {
            _loaded = true;
        }

        private void ShowError()
        {
            _error.gameObject.SetActive(true);
            _error.transform.localScale = Vector3.zero;
            _presentButton.gameObject.SetActive(false);

            var s = DOTween.Sequence();
            s.Append(_error.transform.DOScale(Vector3.one * 2.5f, 1f));
            s.AppendInterval(2f);
            s.AppendCallback(() => { Hide(() => Error?.Invoke()); });
        }

        private void SetPresentSprites(Sprite closed, Sprite opened)
        {
            _closed = closed;
            _opened = opened;

            _presentOpened.sprite = _opened;
            _presentClosed.sprite = _closed;
        }
    }
}