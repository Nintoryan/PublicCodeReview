using UnityEngine;
using VillageGame.UI.Elements;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class CalendarTabSwitcher : TabSwitcher
    {
        [SerializeField] private CalendarPanel _panel;

        private int _activePosition;

        private void OnEnable() => _panel.TabSwitched += OnUpdatePanelContent;

        private void OnDisable() => _panel.TabSwitched += OnUpdatePanelContent;

        private void OnUpdatePanelContent(bool isShop) => _activePosition = isShop ? 1 : 0;

        protected override int GetActivePosition() => _activePosition;
        
        
    }
}