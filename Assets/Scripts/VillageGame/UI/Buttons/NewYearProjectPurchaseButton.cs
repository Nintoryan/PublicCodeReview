using Febucci.UI;
using TMPro;
using UnityEngine;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class NewYearProjectPurchaseButton : UIButton
    {
        [SerializeField] private NewYearProjectsPanel _projectsPanel;
        [SerializeField] private NewYearSelectedProjectPanel _selectedProjectPanel;
        [SerializeField] private TextMeshProUGUI _costText;

        public void SetCostText(string text)
        {
            _costText.text = text;
        }

        public void SetInsufficientCurrency()
        {
            DisableButton();
        }

        protected override void OnClick()
        {
            _projectsPanel.PurchaseProject();
            _projectsPanel.Hide();
        }
    }
}
