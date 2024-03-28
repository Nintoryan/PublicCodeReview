using UnityEngine;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class NewYearProjectSelectButton : UIButton
    {
        [SerializeField] private NewYearProjectSubPanel _newYearProjectSubPanel;

        protected override void OnClick()
        {
            _newYearProjectSubPanel.OnLaunchButtonClick();
        }

        public void Deactivate()
        {
            button.interactable = false;
        }
    }
}
