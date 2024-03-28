using CoreGame.UI.Panels;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace CoreGame.UI.Buttons
{
    public class VictoryReturnUIButton : UIButton
    {
        [SerializeField] private VictoryUIPanel _victoryUIPanel;

        protected override void OnClick()
        {
            _victoryUIPanel.UnloadScene(true);
        }
    }
}
