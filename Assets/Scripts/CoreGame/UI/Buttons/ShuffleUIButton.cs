using CoreGame.UI.Panels;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace CoreGame.UI.Buttons
{
    public class ShuffleUIButton : UIButton
    {
        [SerializeField] private MainUIPanel _mainUIPanel;

        protected override void OnClick()
        {
            _mainUIPanel.ShuffleLevel();
        }
    }
}
