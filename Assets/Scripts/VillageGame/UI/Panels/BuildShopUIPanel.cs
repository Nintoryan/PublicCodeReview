using System;
using System.Collections.Generic;
using System.Linq;
using Data.Scripts.Audio;
using DG.Tweening;
using UnityEngine;
using VillageGame.Data.Types;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class BuildShopUIPanel : UIPanel
    {
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private BuildingsTabUIButton[] _buildingTabs;
        [SerializeField] private BuildShopModal _modal;
        [SerializeField] private GameObject _closeButton;

        private readonly List<ShopBuildingUIButton> _buildingButtons = new();
        
        public List<ShopBuildingUIButton> BuildingButtons => _buildingButtons;
        public BuildShopModal ModalPanel => _modal;
        public ShopBuildingUIButton FirstBuildingButton => _buildingButtons[^1];

        
        public Action<BuildingType> UpdatePanelContentEvent;
        public Action<ShopBuildingUIButton.Data> BuildButtonClickedEvent;
        public Action<ShopBuildingUIButton.Data> ModalAppearedEvent;
        public Action ClickBuildingButtonEvent;
        
        public Transform ContentTransform
        {
            get => _contentTransform;
            set => _contentTransform = value;
        }

        private void Awake()
        {
            _modal.BuildButtonClickedEvent += OnBuildButtonClicked;
            _modal.CloseButtonClickedEvent += OnCloseButtonClicked;
            
            SetFolder(BuildingType.House);
        }

        public void SetFolder(BuildingType type)
        {
            foreach (var tab in _buildingTabs)
            {
                tab.UpdateTab(type);
            }

            UpdatePanelContentEvent?.Invoke(type);
        }

        public void DisableTabsAndCloseButton()
        {
            foreach (var tab in _buildingTabs)
            {
                tab.DisableButton();
            }

            _closeButton.gameObject.SetActive(false);
        }

        public void EnableTabsAndCloseButton()
        {
            foreach (var tab in _buildingTabs)
            {
                tab.EnableButton();
            }

            _closeButton.gameObject.SetActive(true);
        }

        public void ButtonClicked(ShopBuildingUIButton.Data buttonData)
        {
            ClickBuildingButtonEvent?.Invoke();
            _modal.Load(buttonData);
            var s = DOTween.Sequence();
            s.Append(body.DOAnchorPosX(-2000, 0.5f).SetEase(Ease.OutBack));
            s.AppendCallback(() =>
            {
                ModalAppearedEvent?.Invoke(buttonData);
            });
        }

        private void OnBuildButtonClicked(ShopBuildingUIButton.Data buttonData)
        {
            BuildButtonClickedEvent?.Invoke(buttonData);
        }

        private void OnCloseButtonClicked()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.MenuOpen);
            body.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack);
        }

        public void ClearButtons()
        {
            foreach (var button in _buildingButtons)
            {
                button.Hide();
            }
        }

        public void AddButton(ShopBuildingUIButton button)
        {
            _buildingButtons.Add(button);
        }

        public bool IsCreatedButton(BuildingType type, int index)
        {
            return _buildingButtons
                .FirstOrDefault(b => b.CurrentData.BuildingType == type && b.CurrentData.ID == index) != null;
        }

        public bool TryGetCreatedButton(BuildingType type, int index, out ShopBuildingUIButton button)
        {
            button = _buildingButtons
                .FirstOrDefault(b => b.CurrentData.BuildingType == type && b.CurrentData.ID == index);
            return button != null;
        }
    }
}