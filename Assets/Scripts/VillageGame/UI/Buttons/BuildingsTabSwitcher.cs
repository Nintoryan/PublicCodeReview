using System;
using UnityEngine;
using VillageGame.Data.Types;
using VillageGame.UI.Elements;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class BuildingsTabSwitcher : TabSwitcher
    {
        [SerializeField] private BuildShopUIPanel _panel;

        private int _activePosition;

        private void Awake() => _panel.UpdatePanelContentEvent += OnUpdatePanelContent;
        private void OnDestroy() => _panel.UpdatePanelContentEvent += OnUpdatePanelContent;


        private void OnUpdatePanelContent(BuildingType type) => _activePosition = (int)type - 1;

        protected override int GetActivePosition() => _activePosition;
    }
}