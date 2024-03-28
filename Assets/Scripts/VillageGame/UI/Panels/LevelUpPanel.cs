using System.Collections.Generic;
using SO.Data;
using TMPro;
using UnityEngine;
using VillageGame.UI.Buttons;
using VillageGame.UI.Indicators;

namespace VillageGame.UI.Panels
{
    public class LevelUpPanel : UIPanel
    {
        [SerializeField] private TMP_Text _levelNumber;
        [SerializeField] private Transform _unlockedParent;
        [SerializeField] private LevelUpNewItem _newItemPrefab;
        [SerializeField] private EventButton _closeButton;
        [SerializeField] private LevelUIIndicator _levelUIIndicator;
        [SerializeField] private CurrencyUIIndicator _currencyUIIndicator;

        private readonly List<GameObject> _newItems = new();

        private void Awake()
        {
            _closeButton.ClickEvent += HideNoAction;
            PanelSwitchEvent += OnPanelSwitch;
        }
        
        private void OnPanelSwitch(UIPanel _, bool isOn)
        {
            if (isOn)
            {
                _levelUIIndicator.Hide();
                _currencyUIIndicator.Hide();
            }
            else
            {
                _levelUIIndicator.Show();
                _currencyUIIndicator.Show();
            }
        }

        public void SetData(int levelNumber, PlayerLevelData levelData)
        {
            foreach (var newItem in _newItems)
            {
                Destroy(newItem);
            }
            _newItems.Clear();

            foreach (var buildingData in levelData.UnlockedBuildings)
            {
                var item = Instantiate(_newItemPrefab,_unlockedParent);
                item.SetIcon(buildingData.PresentationModel.Icon);
                _newItems.Add(item.gameObject);
            }

            /*foreach (var project in levelData.NewYearProjects)
            {
                var item = Instantiate(_newItemPrefab, _unlockedParent);
                item.SetIcon(project.Icon);
                _newItems.Add(item.gameObject);
            }*/

            _levelNumber.text = levelNumber.ToString();
        }
    }
}