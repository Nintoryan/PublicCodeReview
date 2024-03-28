using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VillageGame.Services;

namespace VillageGame.UI.Buttons
{
    public class RemoveDodoCoinButton : UIButton
    {
        [SerializeField] private GameObject _readyState;
        [SerializeField] private GameObject _loadingState;
        [SerializeField] private GameObject _successState;
        [SerializeField] private GameObject _errorState;
        [SerializeField] private TMP_Text _price;
        
        private Action _successAction;
        private string _idempotencyKey;
        private int _amountToRemove;
        private string _place;

        private void OnEnable()
        {
            SwitchState(DodoCoinButtonState.Ready);
        }

        protected override void OnClick()
        {
            TryRemoveDodoCoins();
        }
        

        public void SetData(int amount, string key,string place, Action success)
        {
            _price.text = amount.ToString();
            _idempotencyKey = key;
            _amountToRemove = amount;
            _successAction = success;
            _place = place;

            button.interactable = amount <= DodoCoinService.CurrentBalance;
        }

        private void TryRemoveDodoCoins()
        {
            if (_idempotencyKey == "" || _amountToRemove == 0 || _successAction == null)
            {
                SwitchState(DodoCoinButtonState.Error);
                return;
            }
            
            SwitchState(DodoCoinButtonState.Loading);
            
            DodoCoinService.WithdrawCoins(_amountToRemove,_idempotencyKey, OnSuccess, OnError);
        }

        private void OnSuccess()
        {
            SwitchState(DodoCoinButtonState.Success);
            DodoAnalyticsService.SendRemovedDodoCoins(_amountToRemove,_place);
            _successAction?.Invoke();
        }

        private void OnError()
        {
            SwitchState(DodoCoinButtonState.Error);
        }

        public void SwitchState(DodoCoinButtonState state)
        {
            if (state == DodoCoinButtonState.Ready)
            {
                EnableButton();
            }
            else
            {
                DisableButton();
            }
            
            _readyState.SetActive(DodoCoinButtonState.Ready == state);
            _loadingState.SetActive(DodoCoinButtonState.Loading == state);
            _successState.SetActive(DodoCoinButtonState.Success == state);
            _errorState.SetActive(DodoCoinButtonState.Error == state);
        }
        
    }

    public enum DodoCoinButtonState
    {
        Ready,
        Loading,
        Success,
        Error
    }
}