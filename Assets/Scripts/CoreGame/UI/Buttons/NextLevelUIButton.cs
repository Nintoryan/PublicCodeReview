using CoreGame.UI.Panels;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace CoreGame.UI.Buttons
{
    public class NextLevelUIButton : UIButton
    {
        [SerializeField] private VictoryUIPanel _victoryUIPanel;

        protected override void OnClick()
        {
            _victoryUIPanel.ReloadScene(true);
        }
    }
}
