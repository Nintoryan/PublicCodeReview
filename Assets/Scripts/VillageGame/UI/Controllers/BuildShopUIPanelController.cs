using System;
using System.Collections.Generic;
using System.Linq;
using SO;
using SO.Data;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data.Types;
using VillageGame.Infrastructure.Factories;
using VillageGame.Services;
using VillageGame.Services.Storages;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Controllers
{
    public class BuildShopUIPanelController : UIPanelController, IInitializable
    {
        private readonly MainUIPanel _mainUIPanel;
        private readonly MainUIPanelController _mainUIPanelController;
        private readonly BuildShopUIPanel _buildShopUIPanel;
        
        private readonly ProgressionService _progressionService;
        private readonly UIFactory _uiFactory;
        private readonly BuildingsConfig _buildingsConfig;
        private readonly ProgressionConfig _progressionConfig;
        
        private readonly PurchasedBuildingsStorage _purchasedBuildingsStorage;
        private readonly RatingStorage _ratingStorage;
        private readonly CurrencyStorage _currencyStorage;

        public Action PressOpenButtonEvent;
        public Action<ShopBuildingUIButton.Data> BuildingButtonClick;

        private BuildingType _buildingType = BuildingType.House;

        public bool IsOpen => _uiPanel.IsActive;
        
        [Inject]
        public BuildShopUIPanelController(IObjectResolver resolver)
        {
            _buildShopUIPanel = resolver.Resolve<UIFacade>().FindPanel<BuildShopUIPanel>();
            _uiPanel = _buildShopUIPanel;
            
            _mainUIPanelController = resolver.Resolve<MainUIPanelController>();
            _mainUIPanel =  resolver.Resolve<UIFacade>().FindPanel<MainUIPanel>();
            _progressionService = resolver.Resolve<ProgressionService>();
            _currencyStorage = resolver.Resolve<CurrencyStorage>();
            _ratingStorage = resolver.Resolve<RatingStorage>();
            _progressionConfig = resolver.Resolve<ProgressionConfig>();
            _uiFactory = resolver.Resolve<UIFactory>();
            _buildingsConfig = resolver.Resolve<BuildingsConfig>();
            _purchasedBuildingsStorage = resolver.Resolve<PurchasedBuildingsStorage>();
        }
        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~BuildShopUIPanelController()
        {
            SubscribeToEvents(false);
        }


        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
               _mainUIPanel.BuildingShopButton.ClickEvent += SwitchPanel;
                _mainUIPanel.BuildingShopButton.ClickEvent += InvokePressButtonEvent;
                _buildShopUIPanel.UpdatePanelContentEvent += OnUpdatePanelContent;
                _buildShopUIPanel.BuildButtonClickedEvent += OnBuildingButtonClicked;
                _ratingStorage.ChangeLevel += OnProgressionUpdate;
                _currencyStorage.Change += OnProgressionUpdate;
            }
            else
            {
                  _mainUIPanel.BuildingShopButton.ClickEvent -= SwitchPanel;
                _mainUIPanel.BuildingShopButton.ClickEvent -= InvokePressButtonEvent;
                _buildShopUIPanel.UpdatePanelContentEvent -= OnUpdatePanelContent;
                _buildShopUIPanel.BuildButtonClickedEvent -= OnBuildingButtonClicked;
                _ratingStorage.ChangeLevel -= OnProgressionUpdate;
                _currencyStorage.Change -= OnProgressionUpdate;
            }
        }
        private void OnProgressionUpdate(int level)
        {
            if (_buildShopUIPanel.IsActive)
            {
                _buildShopUIPanel.SetFolder(_buildingType);
            }
        }
        
        private void InvokePressButtonEvent()
        {
            PressOpenButtonEvent?.Invoke();
        }
        
        public override void ShowUIPanel()
        {
            base.ShowUIPanel();
            _buildShopUIPanel.SetFolder(_buildingType);
        }

        public override void SwitchPanel()
        {
            _buildShopUIPanel.Switch();
            _buildShopUIPanel.SetFolder(_buildingType);
        }

        public void SetType(BuildingType buildingType)
        {
            _buildingType = buildingType;
        }

        private void CreateBuildingButtons(BuildingType buildingType)
        {
            AvailableBuildingsData availableBuildings = _progressionService.GetAvailableBuildings(buildingType);

            _buildShopUIPanel.ClearButtons();

            var targetBuildings = buildingType switch
            {
                BuildingType.House => _buildingsConfig.Houses,
                BuildingType.Decoration => _buildingsConfig.Decorations,
                _ => Array.Empty<BuildingData>()
            };
            
            
            CreateButtons(targetBuildings, availableBuildings.LockedBuildingsIDs, ShopBuildingUIButton.Mode.Locked);
            CreateButtons(targetBuildings, availableBuildings.AvailableBuildingIDs,ShopBuildingUIButton.Mode.Default);
            CreateButtons(targetBuildings, availableBuildings.PurchasedBuildingsIDs, ShopBuildingUIButton.Mode.Purchased);

            SortBuildingButtons();
        }


        private void OnBuildingButtonClicked(ShopBuildingUIButton.Data buttonData)
        {
            BuildingButtonClick?.Invoke(buttonData);
            _buildShopUIPanel.Hide();
        }

        private void OnUpdatePanelContent(BuildingType type)
        {
            _buildingType = type;
            CreateBuildingButtons(type);
        }

        private void CreateButtons(IReadOnlyList<BuildingData> targetBuildings ,IEnumerable<int> IDs,ShopBuildingUIButton.Mode ButtonMode )
        {
            foreach (var id in IDs)
            {
                var buildingData = targetBuildings.FirstOrDefault(b => b.ID == id);

                if (buildingData == null)
                {
                    continue;
                }
                var purchasedBuildingsCount = _buildingType == BuildingType.Decoration &&
                                              ButtonMode == ShopBuildingUIButton.Mode.Purchased
                    ? _purchasedBuildingsStorage.GetDecorationCount(buildingData.ID)
                    : 0;

                var level = 0;
                for (int i = 0; i < _progressionConfig.playerLevels.Length; i++)
                {
                    if (_progressionConfig.playerLevels[i].UnlockedBuildings.Any(b => b.ID == buildingData.ID))
                    {
                        level = i;
                        break;
                    }
                }

                var buttonData = new ShopBuildingUIButton.Data
                {
                    BuildingType = buildingData.Type,
                    ButtonMode = ButtonMode,
                    PurchasedBuildingsCount = purchasedBuildingsCount,
                    ID = buildingData.ID,
                    IsEnoughCurrency = _currencyStorage.IsEnoughCurrency(buildingData.PresentationModel.Price),
                    PresentationModel = buildingData.PresentationModel,
                    UnlockingLevel = level+1
                };
                
                var button = _uiFactory.CreateBuildingButton(buttonData);
                
                button.transform.SetAsFirstSibling();
            }
        }
        
        private void SortBuildingButtons()
        {
            var sortedButtons = _buildShopUIPanel.BuildingButtons.OrderByDescending(button =>
            {
                return button.CurrentData.ButtonMode switch
                {
                    ShopBuildingUIButton.Mode.Purchased => 2,
                    ShopBuildingUIButton.Mode.Default => 1,
                    _ => 0
                };
            }).ThenBy(button => button.CurrentData.ID).ToList();

            foreach (var sortedButton in sortedButtons)
            {
                sortedButton.transform.SetAsLastSibling(); // Используйте SetAsFirstSibling() для изменения порядка на первый
            }
        }
    }
}