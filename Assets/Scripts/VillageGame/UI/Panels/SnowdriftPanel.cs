using TMPro;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class SnowdriftPanel: UIPanel
    {
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private EventButton _buyButton;
        [SerializeField] private EventButton _closeButton;
        public EventButton BuyButton => _buyButton;
        public EventButton CloseButton => _closeButton;

        public void SetPrice(string price)
        {
            _priceText.SetText(price);
        }
    }
}