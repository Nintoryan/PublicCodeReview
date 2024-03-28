using System;
using SO;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Infrastructure.Factories;
using VillageGame.Logic.Buildings;
using VillageGame.Logic.Tutorial;
using VillageGame.Services.CutScenes;
using VillageGame.Services.CutScenes.CustomActions;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Buttons;
using VillageGame.UI.Controllers;

namespace VillageGame.Services.Buildings
{
    public partial class BuildService : IInitializable
    {
        private readonly BuildingFactory _buildingFactory;
        private readonly BuildingsConfig _buildingsConfig;
        private readonly BuildObserver _buildObserver;
        private readonly TimeObserver _timeObserver;

        private readonly TileAreasService _tileAreasService;
        private readonly InputService _inputService;
        private readonly BuildingAreaService _buildingAreaService;
        private readonly ConstructionSiteService _constructionSiteService;

        private readonly PurchasedBuildingsStorage _purchasedBuildingsStorage;
        private readonly BuildingOnMapStorage _buildingsOnMapStorage;
        private readonly CurrencyStorage _currencyStorage;
        private readonly RatingStorage _ratingStorage;

        private readonly BuildShopUIPanelController _buildShopUIPanelController;
        private readonly BuildModeUIPanelController _buildModeUIPanelController;
        private readonly SnowdriftPanelController _snowdriftPanelController;
        private readonly NoDecorationSpacePanelController _noDecorationSpacePanelController;

        private Building _editableBuilding;
        private BuildingAreaData _selectedBuildingArea;
        private BuildingType _buildingType;

        private int _buildingIndex = -1;
        private readonly CutSceneActionsExecutor _cutSceneActionExecutor;
        private readonly TutorialBlackScreen _tutorialBlackScreen;

        private readonly IceCubesService _iceCubesService;

        public Action<Vector3> BuildingPlaceEvent;
        [Inject]
        public BuildService(IObjectResolver objectResolver)
        {
            _buildingFactory = objectResolver.Resolve<BuildingFactory>();
            _buildingsConfig = objectResolver.Resolve<BuildingsConfig>();
            _buildObserver = objectResolver.Resolve<BuildObserver>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();

            _inputService = objectResolver.Resolve<InputService>();
            _tileAreasService = objectResolver.Resolve<TileAreasService>();
            _buildingAreaService = objectResolver.Resolve<BuildingAreaService>();
            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();

            _buildingsOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _purchasedBuildingsStorage = objectResolver.Resolve<PurchasedBuildingsStorage>();
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            
            _buildShopUIPanelController = objectResolver.Resolve<BuildShopUIPanelController>();
            _buildModeUIPanelController = objectResolver.Resolve<BuildModeUIPanelController>();
            _tutorialBlackScreen = objectResolver.Resolve<UIFacade>().TutorialBlackScreen;
            _snowdriftPanelController = objectResolver.Resolve<SnowdriftPanelController>();
            _noDecorationSpacePanelController = objectResolver.Resolve<NoDecorationSpacePanelController>();
            _cutSceneActionExecutor = objectResolver.Resolve<CutSceneActionsExecutor>();

            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        public void Initialize()
        {
            SubscribeToEvent(true);
        }

        ~BuildService()
        {
            SubscribeToEvent(false);
        }

        private void SubscribeToEvent(bool flag)
        {
            if (flag)
            {
                _inputService.PressedTouchEvent += OnPressedTouch;
                _inputService.EndTouchEvent += OnEndPressMouse;
                _inputService.NonBlockStartTouchEvent += OnPressTutorialTouch;
                _buildShopUIPanelController.BuildingButtonClick += BuildingButtonClick;
                _buildShopUIPanelController.PressOpenButtonEvent += Reset;
                _buildModeUIPanelController.PressExitButton += OnPressExitButton;
            }
            else
            {
                _inputService.PressedTouchEvent -= OnPressedTouch;
                _inputService.EndTouchEvent -= OnEndPressMouse;
                _inputService.NonBlockStartTouchEvent -= OnPressTutorialTouch;
                _buildShopUIPanelController.PressOpenButtonEvent -= Reset;
                _buildShopUIPanelController.BuildingButtonClick -= BuildingButtonClick;
                _buildModeUIPanelController.PressExitButton -= OnPressExitButton;
            }
        }

        private void OnPressTutorialTouch(Vector2 position)
        {
            if (_cutSceneActionExecutor.IsWatchingType == CustomCutsceneActionType.SmallSnowdriftTutorial)
            {
                if (_tutorialBlackScreen.IsActive)
                {
                    return;
                }
                if (_buildingAreaService.TryGetBuildingAreaAtPosition(position, out _selectedBuildingArea))
                {
                    if (_selectedBuildingArea.Area.position == new Vector3Int(-11, 1, 0))
                    {
                        if (TryShowSnowdriftsPanel(_selectedBuildingArea)) return;
                    }
                }
            }
            else if (_cutSceneActionExecutor.IsWatchingType == CustomCutsceneActionType.BuildTutorial)
            {
                if (_buildingAreaService.TryGetBuildingAreaAtPosition(position, out _selectedBuildingArea))
                {
                    if (_selectedBuildingArea.Area.position == new Vector3Int(-11, 1, 0))
                    {
                        if (_buildingType == BuildingType.None)
                        {
                            _buildingType = _selectedBuildingArea.Type;
                        }

                        if (TryBuild(_selectedBuildingArea)) return;
                    }
                }
            }
        }


        private void OnPressedTouch(Vector2 position)
        {
            if (!_inputService.IsPressedOnePosition)
            {
                return;
            }

            if (_buildingAreaService.TryGetBuildingAreaAtPosition(position, out _selectedBuildingArea))
            {
                if (TrySetEditableBuildings(_selectedBuildingArea))
                {
                    _buildObserver.InvokeStartEvent(_editableBuilding.Data.Type);
                    //_buildObserver.InvokeStartEvent();
                }
            }
        }

        private void OnEndPressMouse(Vector2 position, float pressTime)
        {
            if (_inputService.IsBlock || Constance.IsWatchingCinema() ||
                pressTime > 0.1f && !_inputService.IsPressedOnePosition) return;

            if (_buildingAreaService.TryGetBuildingAreaAtPosition(position, out _selectedBuildingArea))
            {
                if (_buildingType == BuildingType.None)
                {
                    _buildingType = _selectedBuildingArea.Type;
                }

                if (_selectedBuildingArea.Type == BuildingType.IceCube)
                {
                    _iceCubesService.ActivateIceCube();
                    _buildingType = BuildingType.None;
                    return;
                }

                if (TryShowSnowdriftsPanel(_selectedBuildingArea)) return;
                if (TryShowShopBuildingPanel(_selectedBuildingArea)) return;
                if (TryBuild(_selectedBuildingArea)) return;
                if (TryMoveBuilding(_selectedBuildingArea)) return;
            }
        }


        public void OnEndTutorialPress(Vector2 position, float pressTime)
        {
            if (pressTime > 0.1f && !_inputService.IsPressedOnePosition) return;

            if (_buildingAreaService.TryGetBuildingAreaAtPosition(position, out _selectedBuildingArea))
            {
                if (TryShowSnowdriftsPanel(_selectedBuildingArea)) return;
            }
        }

        private void BuildingButtonClick(ShopBuildingUIButton.Data buttonData)
        {
            var siteCount = _constructionSiteService.GetClearConstructionSiteCount();
            if (buttonData.BuildingType == BuildingType.House && siteCount == 0)
            {
                _snowdriftPanelController.ShowBuildWarningPanel();
                Reset();
                _buildObserver.InvokeStopEvent();
                return;
            }

            if (buttonData.BuildingType == BuildingType.Decoration && !_buildingAreaService.HasFreeDecorationArea())
            {
                _noDecorationSpacePanelController.ShowNoSpaceWarningPanel();
                Reset();
                _buildObserver.InvokeStopEvent();
                return;
            }

            _buildingIndex = buttonData.ID;
            _buildingType = buttonData.BuildingType;
            var build = _buildingsConfig.GetData(_buildingType, _buildingIndex);
            _buildingAreaService.ShowClueAreas(build.Type);

            TryBuild(_selectedBuildingArea);
        }

        private void Reset()
        {
            _buildObserver.InvokeStopEvent();
            _buildingIndex = -1;
            _buildingAreaService.HideClueAreas();
            _buildingType = BuildingType.None;
            _editableBuilding?.CanvasController.HideAllButtons();
            _editableBuilding = null;
            _selectedBuildingArea = null;
        }
    }
}