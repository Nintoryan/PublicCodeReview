using System;
using TMPro;
using UnityEngine;
using VContainer;
using VillageGame.Services.Storages;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels.Warnings;

namespace VillageGame.UI.Panels
{
    public class IceCubePanel : UIPanel
    {
        [SerializeField] private EventButton _closeButton;
        [SerializeField] private EventButton _buyButton;
        [SerializeField] private int _removePrice;
        [SerializeField] private TextMeshProUGUI _priceText;

        private CurrencyStorage _currencyStorage;
        private NoStarsPanel _noStarsPanel;

        public Action IceCubeRemovedEvent;

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            _noStarsPanel = objectResolver.Resolve<UIFacade>().FindPanel<NoStarsPanel>();
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
            _priceText.text = _removePrice.ToString();
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _closeButton.ClickEvent += OnCloseButtonClickEvent;
                _buyButton.ClickEvent += OnBuyButtonClickEvent;
            }
            else
            {
                _closeButton.ClickEvent -= OnCloseButtonClickEvent;
                _buyButton.ClickEvent -= OnBuyButtonClickEvent;
            }
        }

        private void OnCloseButtonClickEvent()
        {
            Hide();
        }

        private void OnBuyButtonClickEvent()
        {
            if (_currencyStorage.IsEnoughCurrency(_removePrice))
            {
                _currencyStorage.Remove(_removePrice);
                IceCubeRemovedEvent?.Invoke();
                Hide();
            }
            else
            {
                Hide();
                _noStarsPanel.Show();
            }
        }
    }
}
