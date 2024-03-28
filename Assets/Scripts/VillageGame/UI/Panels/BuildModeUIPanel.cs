using UnityEngine;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class BuildModeUIPanel : UIPanel
    {
        [SerializeField] private EventButton _exitBuildModeButton;
        public EventButton ExitModeButton => _exitBuildModeButton;
    }
}
