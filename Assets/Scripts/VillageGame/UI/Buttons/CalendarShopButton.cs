using System;
using SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Services;

namespace VillageGame.UI.Buttons
{
    public class CalendarShopButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private Image _giftImage;
        
        
        public EventButton _show;
        public GameObject _loading;
        public GameObject _error;
        public GameObject _success;
        public EventButton _buy;

        public int GiftId { get; private set; } = -1;
        private DodoBirdGift _currentData;
        public string idempotencyKey;


        private void OnEnable()
        {
            DodoBirdsService.BalanceUpdated += OnBalanceChange;
            OnBalanceChange();
        }

        private void OnDisable()
        {
            DodoBirdsService.BalanceUpdated -= OnBalanceChange;
        }

        public void SetData(DodoBirdGift data, bool isBought, Action<CalendarShopButton> onClick)
        {
            _currentData = data;
            _success.gameObject.SetActive(false);
            _error.SetActive(false);
            _loading.SetActive(false);
            
            GiftId = data.Id;
            _giftImage.sprite = isBought ? data.Opened : data.Closed;
            _title.SetText(data.Title);
            _price.SetText(data.Price.ToString());
            _show.gameObject.SetActive(isBought);
            _buy.gameObject.SetActive(!isBought);

            _show.ClickEvent += () => onClick?.Invoke(this);
            _buy.ClickEvent += () => onClick?.Invoke(this);

            if (!isBought)
            {
                idempotencyKey = Extensions.GenerateRandomIdempotencyKey();
            }

            OnBalanceChange();
        }

        private void OnBalanceChange()
        {
            if(_currentData == null) return;
            
            if (DodoBirdsService.Balance < _currentData.Price)
            {
                _buy.DisableButton();
            }
            else
            {
                _buy.EnableButton();
            }
        }

        public void ShowLoading()
        {
            _show.gameObject.SetActive(false);
            _buy.gameObject.SetActive(false);
            _success.gameObject.SetActive(false);
            _error.SetActive(false);
            _loading.SetActive(true);
        }
        
        public void ShowError()
        {
            _show.gameObject.SetActive(false);
            _buy.gameObject.SetActive(false);
            _success.gameObject.SetActive(false);
            _error.SetActive(true);
            _loading.SetActive(false);
        }

        public void ShowSuccess()
        {
            _show.gameObject.SetActive(false);
            _buy.gameObject.SetActive(false);
            _success.gameObject.SetActive(true);
            _error.SetActive(false);
            _loading.SetActive(false);
        }

    }
}