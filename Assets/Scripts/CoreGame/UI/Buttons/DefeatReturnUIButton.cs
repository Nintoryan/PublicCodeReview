using CoreGame.UI.Panels;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace CoreGame.UI.Buttons
{
    public class DefeatReturnUIButton : UIButton
    {
        [SerializeField] private DefeatUIPanel _defeatUIPanel;

        protected override void OnClick()
        {
            _defeatUIPanel.UnloadScene(false);
        }
    }
}
