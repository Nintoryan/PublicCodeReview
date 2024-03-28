using Data.Scripts.Audio;
using System;
using System.Globalization;
using UnityEngine;
using Util;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Logic.Buildings;
using VillageGame.Logic.Snowdrifts;
using Web.ResponseStructs.PayloadValues;

namespace VillageGame.Services.Buildings
{
    public partial class BuildService
    {
        private void RemoveBuilding()
        {
            if (_editableBuilding == null)
            {
                return;
            }
            
            TryGetAreaUnderEditableBuilding(out var area);
            area.ResetBuild(isShowHideClueAre: false);
            _constructionSiteService.SetConstructionSiteState(area.Area.position, ConstructionSite.StateType.ConstructionSite);
            
            _buildingsOnMapStorage.RemoveBuilding(_editableBuilding);
            _purchasedBuildingsStorage.Add(_editableBuilding.Data.Type, _editableBuilding.Data.ID);
            _editableBuilding.DestroyYourself();

            Reset();
        }

        private void AcceptBuild()
        {
            if (_editableBuilding == null || _selectedBuildingArea == null && !TryGetAreaUnderEditableBuilding(out _selectedBuildingArea))
            {
                return;
            }

            if (_selectedBuildingArea.Type != _buildingType)
            {
                return;
            }

            _editableBuilding.CanvasController.HideAllButtons();
            _buildingAreaService.HideClueAreas();

            if (_buildingsOnMapStorage.IsBuildUp(_editableBuilding))
            {
                var buildData = _editableBuilding.BuildData;

                buildData.x = _selectedBuildingArea.Area.x;
                buildData.y = _selectedBuildingArea.Area.y;

                _editableBuilding.SetBuildData(buildData);
                _buildingsOnMapStorage.RefreshBuilding(_editableBuilding);
            }
            else
            {
                var buildData = new BuildData()
                {
                    id = _buildingIndex,
                    type = _buildingType,
                    x = _selectedBuildingArea.Area.x,
                    y = _selectedBuildingArea.Area.y,
                    build_time = _timeObserver.CurrentPlayerTime.ToString(CultureInfo.InvariantCulture),
                };

                _editableBuilding.SetBuildData(buildData);
                _editableBuilding.Mainer.SetMaxCooldown();

                _buildingsOnMapStorage.TryAddBuilding(_editableBuilding);
                if (!_purchasedBuildingsStorage.TryRemove(_editableBuilding.Data.Type, _editableBuilding.Data.ID))
                {
                    _currencyStorage.Remove(_editableBuilding.Data.PresentationModel.Price);
                    _ratingStorage.AddRating(_editableBuilding.Data.PresentationModel.RatingAfterConstruction);
                }
            }

            BuildingPlaceEvent?.Invoke(_editableBuilding.transform.position);
            Reset();
        }


        private bool TrySetEditableBuildings(BuildingAreaData area)
        {
            if (/*_editableBuilding != null || */area == null) return false;
            if (area.IsBuildUp && _inputService.IsPressedOnePosition)
            {
                var newEditableBuilding = _buildingsOnMapStorage.GetBuilding(area.Area.x, area.Area.y);
                if (newEditableBuilding == null)
                {
                    return false;
                }

                if(_editableBuilding != null)
                {
                    _editableBuilding.CanvasController.HideAllButtons();

                    if (newEditableBuilding.Data.Type != _editableBuilding.Data.Type)
                    {
                        if (_editableBuilding.Data.Type == BuildingType.House)
                        {
                            _constructionSiteService.SetHighlightConstructionSites(false);
                        }
                        else
                        {
                            _buildingAreaService.HideClueAreas();
                        }
                    }
                }
                
                _editableBuilding = newEditableBuilding;
                _buildingType = _editableBuilding.Data.Type;
                _buildingIndex = _editableBuilding.Data.ID;
             
                var buildingArea = _selectedBuildingArea;
                _editableBuilding.CanvasController.ShowDeleteButton(RemoveBuilding);
                _editableBuilding.CanvasController.ShowBuildButton(AcceptBuild);
                _buildingAreaService.ShowClueAreas(_editableBuilding.Data.Type);
                return true;
            }

            return false;
        }

        private bool TryBuild(BuildingAreaData buildingAreaData)
        {
            if (buildingAreaData == null || buildingAreaData.Type != _buildingType)
            {
                _buildObserver.InvokeStartEvent(_buildingType);
                return false;
            }

            if (_constructionSiteService.TryGetSnowdrift(buildingAreaData.Area.position, out _))
            {
                return false;
            }


            if (_editableBuilding == null && _buildingIndex > -1 && !buildingAreaData.IsBuildUp)
            {
                var buildingData = _buildingsConfig.GetData(_buildingType, _buildingIndex);
                if (buildingAreaData.Type == buildingData.Type)
                {
                    var buildingPosition = _tileAreasService.CellToLocalPosition(buildingAreaData.Area.position);
                    _editableBuilding = _buildingFactory.InstantiateBuildings(_buildingType, _buildingIndex, buildingPosition);
                    _constructionSiteService.SetConstructionSiteState(buildingAreaData.Area.position, ConstructionSite.StateType.None);
                    AudioManager.Instance.PlayAudioEvent(_buildingType == BuildingType.House ? AudioEventType.HousePlacing : AudioEventType.DecorPlacing);
                    buildingAreaData.SetBuild(_buildingIndex);

                    var buildData = new BuildData
                    {
                        x = buildingAreaData.Area.x,
                        y = buildingAreaData.Area.y
                    };
                    _editableBuilding.SetBuildData(buildData);
                    
                    AcceptBuild();
                }

                return true;
            }

            return false;
        }

        private bool TryShowSnowdriftsPanel(BuildingAreaData buildingAreaData)
        {
            var notEditBuild = _buildingIndex == -1 && _editableBuilding == null;
            if (notEditBuild && _constructionSiteService.TryGetSnowdrift(buildingAreaData.Area.position, out var site))
            {
                _snowdriftPanelController.ShowPanel(site);
                _selectedBuildingArea = null;
                Reset();
                return true;
            }

            return false;
        }

        private bool TryShowShopBuildingPanel(BuildingAreaData buildingAreaData)
        {
            if (_buildShopUIPanelController.IsOpen)
            {
                return false;
            }

            if (_buildingIndex == -1 && _editableBuilding == null && !buildingAreaData.IsBuildUp)
            {
                _buildShopUIPanelController.SetType(buildingAreaData.Type);
                _buildShopUIPanelController.ShowUIPanel();
                return true;
            }

            return false;
        }

        private bool TryMoveBuilding(BuildingAreaData buildingAreaData)
        {
            if (_constructionSiteService.TryGetSnowdrift((Vector2Int)buildingAreaData.Area.position, out var site))
            {
                return false;
            }

            if (_editableBuilding != null && buildingAreaData.Type == _buildingType && !buildingAreaData.IsBuildUp)
            {
                if (TryGetAreaUnderEditableBuilding(out var currentBuildingArea))
                {
                    currentBuildingArea.ResetBuild();
                    _constructionSiteService.SetConstructionSiteState(currentBuildingArea.Area.position,
                        ConstructionSite.StateType.ConstructionSite);
                }

                var buildingPosition = _tileAreasService.CellToLocalPosition(buildingAreaData.Area.position);
                _editableBuilding.transform.position = buildingPosition + Constance.GetBuildingOffset(_buildingType);

                var buildData = _editableBuilding.BuildData;
                buildData.x = _selectedBuildingArea.Area.x;
                buildData.y = _selectedBuildingArea.Area.y;
                _editableBuilding.SetBuildData(buildData);

                buildingAreaData.SetBuild(_editableBuilding.Data.ID);
                _constructionSiteService.SetConstructionSiteState(buildingAreaData.Area.position,
                    ConstructionSite.StateType.None);
                AudioManager.Instance.PlayAudioEvent(AudioEventType.ObjectMove);

                if (_buildingType == BuildingType.House)
                {
                    _constructionSiteService.SetHighlightConstructionSites(true);
                }
                
                return true;
            }
                AudioManager.Instance.PlayAudioEvent(AudioEventType.ObjectGrab);
            return false;
        }

        private void OnPressExitButton()
        {
            if (_editableBuilding != null && !_buildingsOnMapStorage.IsBuildUp(_editableBuilding))
            {
                var currentX = _editableBuilding.BuildData.x;
                var currentY = _editableBuilding.BuildData.y;
                if (_buildingAreaService.TryGetBuildingAreaByCoordinate(currentX, currentY,
                        out var currentBuildingArea))
                {
                    currentBuildingArea.ResetBuild();
                    _constructionSiteService.SetConstructionSiteState(currentBuildingArea.Area.position,
                        ConstructionSite.StateType.ConstructionSite);
                }

                _editableBuilding.DestroyYourself();
            }

            Reset();
        }

        private bool TryGetAreaUnderEditableBuilding(out BuildingAreaData areaData)
        {
            var currentX = _editableBuilding.BuildData.x;
            var currentY = _editableBuilding.BuildData.y;
            return _buildingAreaService.TryGetBuildingAreaByCoordinate(currentX, currentY, out areaData);
        }
    }
}