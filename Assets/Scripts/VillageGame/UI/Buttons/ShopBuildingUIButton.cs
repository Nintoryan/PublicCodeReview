using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Data;
using VillageGame.Data.PresentationModels;
using VillageGame.Data.Types;
using VillageGame.UI.Elements;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class ShopBuildingUIButton : UIButton
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _nameLockedText;
        [SerializeField] private TMP_Text _unlockedLevel;
        [SerializeField] private TextPanel _pricePanel;
        [SerializeField] private TextPanel _countPanel;
        [SerializeField] private Image _iconHouse;
        [SerializeField] private Image _iconDecoration;
        [SerializeField] private GameObject _lockPanel;
        [SerializeField] private Color _enoughCurrencyColor = Color.white;
        [SerializeField] private Color _notEnoughCurrencyColor = Color.red;

        private BuildShopUIPanel _buildUIPanel;
        public Data CurrentData { get; private set; }

        public void Initialize(BuildShopUIPanel buildUIPanel, Data data)
        {
            CurrentData = data;
            _buildUIPanel = buildUIPanel;

            switch (data.ButtonMode)
            {
                case Mode.Locked:
                    SetLockedMode();
                    break;
                case Mode.Default:
                default:
                    SetDefaultMode();
                    break;
                case Mode.Purchased:
                    SetPurchasedMode();
                    break;
            }
        }

        protected override void OnClick()
        {
            _buildUIPanel.ButtonClicked(CurrentData);
            if (CurrentData.ButtonMode == Mode.Purchased)
            {
                CurrentData.PurchasedBuildingsCount--;
                if (CurrentData.PurchasedBuildingsCount == 0)
                {
                    CurrentData.ButtonMode = Mode.Default;
                    SetDefaultMode();
                }
            }
        }

        private void SetLockedMode()
        {
            DisableButton();
            _nameLockedText.SetText(CurrentData.PresentationModel.Name);
            _countPanel.DisablePanel();
            _unlockedLevel.text = CurrentData.UnlockingLevel.ToString();
        }

        private void SetDefaultMode()
        {
            EnableButton();
            _pricePanel.EnablePanel();
            _pricePanel.SetText(CurrentData.PresentationModel.Price.ToString());
            _pricePanel.SetTextColor(CurrentData.IsEnoughCurrency ? _enoughCurrencyColor : _notEnoughCurrencyColor);
            _iconHouse.color = CurrentData.IsEnoughCurrency ? Color.white : DodoColors.HexToColor("646464", 0.5f);
            _iconDecoration.color = CurrentData.IsEnoughCurrency ? Color.white : DodoColors.HexToColor("646464", 0.5f);
            _countPanel.DisablePanel();
            button.interactable = CurrentData.IsEnoughCurrency;
        }

        private void SetPurchasedMode()
        {
            EnableButton();
            _pricePanel.DisablePanel();

            if (CurrentData.BuildingType == BuildingType.Decoration)
            {
                _countPanel.EnablePanel();
                _countPanel.SetText($"х{CurrentData.PurchasedBuildingsCount}");
            }
            else
            {
                _countPanel.DisablePanel();
            }
        }

        public override void DisableButton()
        {
            base.DisableButton();
            _nameLockedText.SetText(CurrentData.PresentationModel.Name);
            _unlockedLevel.text = CurrentData.UnlockingLevel.ToString();
            _lockPanel.SetActive(true);
        }

        public override void EnableButton()
        {
            base.EnableButton();
            _lockPanel.SetActive(false);

            if (CurrentData.BuildingType == BuildingType.House)
            {
                _iconDecoration.gameObject.SetActive(false);
                _iconHouse.gameObject.SetActive(true);
                _iconHouse.sprite = CurrentData.PresentationModel.Icon;
            }

            if (CurrentData.BuildingType == BuildingType.Decoration)
            {
                _iconDecoration.gameObject.SetActive(true);
                _iconHouse.gameObject.SetActive(false);
                _iconDecoration.sprite = CurrentData.PresentationModel.Icon;
            }
            
            _unlockedLevel.text = CurrentData.UnlockingLevel.ToString();
            _nameText.SetText(CurrentData.PresentationModel.Name);
            _nameLockedText.SetText(CurrentData.PresentationModel.Name);
          
        }
        
        [Serializable]

        public class Data
        {
            public BuildingType BuildingType;
            public int UnlockingLevel;
            public int ID;
            public BuildingPresentationModel PresentationModel;
            public Mode ButtonMode;
            public bool IsEnoughCurrency;
            public int PurchasedBuildingsCount;
        }

        public enum Mode
        {
            Default,
            Locked,
            Purchased
        }
    }
}