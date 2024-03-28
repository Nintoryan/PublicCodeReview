using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Data;
using VillageGame.UI.Buttons;
using VillageGame.UI.Indicators;

namespace VillageGame.UI.Panels
{
    public class CalendarPanel : UIPanel
    {
        [SerializeField] private Transform buttonsContentTransform;
        [SerializeField] private List<CalendarPresentButton> _presentButtons;
        [SerializeField] private LevelUIIndicator _levelUIIndicator;
        [SerializeField] private CurrencyUIIndicator _currencyUIIndicator;
        [SerializeField] private GameObject _shop;
        [SerializeField] private GameObject _myPresents;
        [SerializeField] private GameObject _closeButton;
        [SerializeField] private List<CalendarTab> _tabs;
        [SerializeField] private CalendarShop _calendarShop;
        [SerializeField] private TMP_Text _dodoBirdCounter;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Transform _downEmptyField;
        public CalendarShop CalendarShop => _calendarShop;
        public List<CalendarPresentButton> Buttons => _presentButtons;
        public Transform ContentTransform => buttonsContentTransform;
        
        public Action<bool> TabSwitched;

        public void RefreshPresentButtons(DayData currentDay)
        {
            foreach (var presentButton in _presentButtons)
            {
                presentButton.Refresh(currentDay);
            }  
        }
        public override void Show(Action onShown = null)
        {
            base.Show(onShown);
            OpenMyPresents();
        }
        
        public void LoadDodoBirdCounter(int amount)
        {
            _dodoBirdCounter.text = amount.ToString();
        }
        
        public void DisableTabsAndCloseButton()
        {
            foreach (var tab in _tabs)
            {
                tab.DisableButton();
            }
            _closeButton.SetActive(false);
            _scrollRect.enabled = false;
        }
        
        public void EnableTabsAndCloseButton()
        {
            foreach (var tab in _tabs)
            {
                tab.EnableButton();
            }
            _closeButton.SetActive(true);
            _scrollRect.enabled = true;
        }
        

        public void AddNewPresentButton(CalendarPresentButton button)
        {
            _presentButtons.Add(button);
            _downEmptyField.SetAsLastSibling();
        }

        private void OpenShop()
        {
            _calendarShop.SelectShop();
            TabSwitched?.Invoke(true);
            _shop.gameObject.SetActive(true);
            _myPresents.gameObject.SetActive(false);
            
            foreach (var tab in _tabs)
            {
                tab.UpdateTab(true);
            }
        }

        private void OpenMyPresents()
        {
            TabSwitched?.Invoke(false);
            _shop.gameObject.SetActive(false);
            _myPresents.gameObject.SetActive(true);
            foreach (var tab in _tabs)
            {
                tab.UpdateTab(false);
            }
        }

        private void Start()
        {
            PanelSwitchEvent += OnPanelSwitch;
        }

        private void OnPanelSwitch(UIPanel _, bool isOn)
        {
            if (isOn)
            {
                _levelUIIndicator.Hide();
                _currencyUIIndicator.Hide();
            }
            else
            {
                _levelUIIndicator.Show();
                _currencyUIIndicator.Show();
            }
        }

        public void SetFolder(bool isShop)
        {
            if (isShop)
            {
                OpenShop();
            }
            else
            {
                OpenMyPresents();
            }
            
            foreach (var tab in _tabs)
            {
                tab.UpdateTab(isShop);
            }
        }

        public CalendarPresentButton GetFirstPresentButton()
        {
            return _presentButtons.Count > 0 ? _presentButtons[0] : null;
        }

       
    }
}