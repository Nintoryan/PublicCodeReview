using System;
using UnityEngine;
using VillageGame.Services;
using Web.ResponseStructs;

namespace VillageGame.UI.Buttons
{
    public class AddDodoCoinButton : UIButton
    {
        [SerializeField] private GameObject _readyState;
        [SerializeField] private GameObject _loadingState;
        [SerializeField] private GameObject _successState;
        [SerializeField] private GameObject _errorState;
        
        private Action _successAction;
        private string _idempotencyKey;
        private AddCoinsType _addCoinsType = AddCoinsType.none;
        private string _place;
        private void OnEnable()
        {
            SwitchState(DodoCoinButtonState.Ready);
        }

        protected override void OnClick()
        {
            TryAddDodoCoins();
        }

        public void SetData(AddCoinsType addType, string key, string place, Action success)
        {
            _idempotencyKey = key;
            _addCoinsType = addType;
            _successAction = success;
            _place = place;
        }

        private void TryAddDodoCoins()
        {
            if (_idempotencyKey == "" || _addCoinsType ==  AddCoinsType.none || _successAction == null)
            {
                SwitchState(DodoCoinButtonState.Error);
                return;
            }
            
            SwitchState(DodoCoinButtonState.Loading);
            
            DodoCoinService.AddCoins(_addCoinsType,_idempotencyKey, OnSuccess, OnError);
        }

        private void OnSuccess()
        {
            SwitchState(DodoCoinButtonState.Success);
            DodoAnalyticsService.SendAddDodoCoins(_addCoinsType,_place);
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

}