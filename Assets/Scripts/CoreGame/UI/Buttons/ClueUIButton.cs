using CoreGame.UI.Panels;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace CoreGame.UI.Buttons
{
    public class ClueUIButton : UIButton
    {
        [SerializeField] private MainUIPanel _mainUIPanel;

        protected override void OnClick()
        {
            _mainUIPanel.ShowClue();
        }
    }
}
