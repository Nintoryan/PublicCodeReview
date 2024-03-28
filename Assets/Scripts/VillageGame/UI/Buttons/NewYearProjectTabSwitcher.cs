using UnityEngine;
using VillageGame.UI.Elements;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class NewYearProjectTabSwitcher : TabSwitcher
    {
        [SerializeField] private NewYearProjectsPanel _panel;

        private int _activePosition;

        private void OnEnable() => _panel.TabSwitched += OnUpdatePanelContent;

        private void OnDisable() => _panel.TabSwitched += OnUpdatePanelContent;

        private void OnUpdatePanelContent(bool isNew) => _activePosition = isNew ? 0 : 1;

        protected override int GetActivePosition() => _activePosition;
    }
}