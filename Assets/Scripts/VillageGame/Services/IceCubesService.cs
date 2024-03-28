using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.Logic.Buildings;
using VillageGame.Services.Buildings;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Panels;
using Web.RequestStructs;
using Web.ResponseStructs;

namespace VillageGame.Services
{
    public class IceCubesService : MonoBehaviour, ILoading
    {
        private IceCubePanel _iceCubePanel;
        private BuildingOnMapStorage _buildingsOnMapStorage;

        [SerializeField] private List<IceCubeAreaData> _iceCubesAreas;

        private int _buildingId = 18;
        private int _iceCubesRemoved = 0;
        private BuildingWithCharacter _building;
        private Animator _characterAnimator;
        private bool _isActive;
        private int _lastSelectedIceCubeId = -1;
        private bool[] _iceCubesStatus = new bool[10];

        public Action<int> IceCubeRemovedEvent;
        public Action<IceCubesData> IceCubesDataUpdatedEvent;

        public bool IsActive { get => _isActive; set => _isActive =  value ; }

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _buildingsOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _iceCubePanel = objectResolver.Resolve<UIFacade>().FindPanel<IceCubePanel>();
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _buildingsOnMapStorage.BuildBuildingEvent += OnBuildBuildingEvent;
                _iceCubePanel.IceCubeRemovedEvent += OnIceCubeRemovedEvent;
            }
            else
            {
                _buildingsOnMapStorage.BuildBuildingEvent -= OnBuildBuildingEvent;
                _iceCubePanel.IceCubeRemovedEvent -= OnIceCubeRemovedEvent;
            }
        }

        public void DestroyBuilding()
        {
            if (_building != null)
            {
                Invoke(nameof(DestroyWithDelay), 2f);
            }
        }

        private void DestroyWithDelay()
        {
            _building.ActiveSprite(false);
            foreach (var iceCube in _iceCubesAreas)
            {
                iceCube.IceCubeObject.gameObject.SetActive(true);
            }
        }

        public void RestoreBuilding()
        {
            if (_building != null)
            {
                Invoke(nameof(RestoreWithDelay), 2f);
            }
        }

        private void RestoreWithDelay()
        {
            _building.ActiveSprite(true);
        }

        public void ActivateCharacter()
        {
            _characterAnimator.SetTrigger("CharacterTrigger");
        }

        public void DeactivateCharacter()
        {
            _building.transform.GetChild(3).gameObject.SetActive(false);
        }

        public bool CheckPositionForIceCube(Vector3Int position)
        {
            foreach (var area in _iceCubesAreas)
            {
                if (area.Area.Contains(position) && !area.IsCleared)
                {
                    _lastSelectedIceCubeId = area.Id;
                    return true;
                }
            }
            return false;
        }

        private void OnIceCubeRemovedEvent()
        {
            _iceCubesRemoved++;

            if(_iceCubesRemoved == _iceCubesAreas.Count)
            {
                _isActive = false;
            }
            _iceCubesAreas[_lastSelectedIceCubeId].IceCubeObject.gameObject.SetActive(false);
            _iceCubesAreas[_lastSelectedIceCubeId].IsCleared = true;
            IceCubeRemovedEvent?.Invoke(_iceCubesRemoved);

            _iceCubesStatus[_lastSelectedIceCubeId] = true;
            IceCubesData iceCubesData = new(_isActive, _iceCubesStatus);
            IceCubesDataUpdatedEvent?.Invoke(iceCubesData);
        }

        private void OnBuildBuildingEvent(BuildingType type, int index)
        {
            if (type == BuildingType.House && index == _buildingId)
            {
                _isActive = true;
                _buildingsOnMapStorage.TryGetBuilding(type, index, out Building building);
                _building = building as BuildingWithCharacter;
                _characterAnimator = _building.CharacterAnimator;
                IceCubesData iceCubesData = new(true, new bool[10]);
                IceCubesDataUpdatedEvent?.Invoke(iceCubesData);
            }
        }

        public void ActivateIceCube()
        {
            _iceCubePanel.Show();
        }

        public Vector3 GetPositionOfSelectedIceCube()
        {
            return _iceCubesAreas[_lastSelectedIceCubeId].IceCubeObject.transform.position;
        }

        public BuildingLookPosition GetSelectedIceCubeLookPosition()
        {
            return _iceCubesAreas[_lastSelectedIceCubeId].IceCubeObject.GetComponent<IceCube>().LookPosition;
        }

        public void Load(LoadData request)
        {
            if (request.data.ice_cubes_data.active)
            {
                _isActive = true;
                _buildingsOnMapStorage.TryGetBuilding(BuildingType.House, 18, out Building building);
                _building = building as BuildingWithCharacter;
                _characterAnimator = _building.CharacterAnimator;
                _building.ActiveSprite(false);
                for (int i = 0; i < _iceCubesAreas.Count; i++)
                {
                    IceCubeAreaData iceCube = _iceCubesAreas[i];
                    if(request.data.ice_cubes_data.cleared_cubes[i] == true)
                    {
                        iceCube.IceCubeObject.gameObject.SetActive(false);
                        _iceCubesRemoved++;
                    }
                    else
                    {
                        iceCube.IceCubeObject.gameObject.SetActive(true);
                    }
                }
                _iceCubesStatus = request.data.ice_cubes_data.cleared_cubes;
            }
            else
            {
                foreach (var iceCube in _iceCubesAreas)
                {
                    iceCube.IceCubeObject.gameObject.SetActive(false);
                }
            }
        }
    }

    [Serializable]
    public class IceCubeAreaData
    {
        public int Id;
        public BoundsInt Area;
        public GameObject IceCubeObject;
        public bool IsCleared;
    }
}
