using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.Data;
using Util;
using Data.Scripts.Audio;

namespace CoreGame
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private ObjectDragger _objectDragger;
        [SerializeField] private LevelStorage _levelStorage;

        [SerializeField] private Grid _grid;
        [SerializeField] private GameObject _gridObjectPrefab;
        [SerializeField] private GameObject _megaGridObjectPrefab;
        [SerializeField] private GameObject _superMegaGridObjectPrefab;
        [SerializeField] private GameObject _subgridObjectPrefab;

        [SerializeField] private List<GridPositionData> _gridObjects;

        [SerializeField] private Vector2Int _selectedGridPosition;
        [SerializeField] private Vector2Int _selectedSubgridPosition;
        [SerializeField] private GridObject _hoveredGridObject;
        [SerializeField] private bool _isHoverOverPerson;
        private bool _isObjectSelected;

        private const float _forwardLayerOffset = 0.25f;
        private const float _backLayerOffset = 1.5f;
        private const float _verticalShift = 5.0f;
        private const int _defaultSubgridObjectLayer = 3;
        private const int _draggedObjectLayer = 12;

        public Action<Vector3> MergeObjectEvent;
        public Action<Vector3> UnpackObjectEvent;
        public Action<Vector3> BirdMergedEvent;
        public Action<Transform> BirdSpawnedEvent;

        public GridObject HoveredGridObject { get => _hoveredGridObject; set => _hoveredGridObject = value; }
        public bool IsHoverOverPerson { get => _isHoverOverPerson; set => _isHoverOverPerson = value; }

        public void ShiftGridPosition()
        {
            _grid.transform.position += Vector3.down * _verticalShift;
        }

        public void AddGridObject(Vector2Int gridPosition, ContainerType containerType, bool isEnabled, bool isLocked, int lockCounter)
        {
            GameObject instance;
            Vector3 position = _grid.CellToWorld((Vector3Int)gridPosition);

            switch (containerType)
            {
                case ContainerType.MegaContainer:
                    instance = Instantiate(_megaGridObjectPrefab, transform);
                    instance.transform.position = position + Vector3.left * 2.6f;
                    instance.name = gridPosition.ToString() + " MegaObject";
                    break;
                case ContainerType.SuperMegaContainer:
                    instance = Instantiate(_superMegaGridObjectPrefab, transform);
                    instance.transform.position = position + Vector3.left * 7.8f;
                    instance.name = gridPosition.ToString() + " SuperMegaObject";
                    break;
                default:
                    instance = Instantiate(_gridObjectPrefab, transform);
                    instance.transform.position = position;
                    instance.name = gridPosition.ToString();
                    break;
            }

            GridPositionData gridPositionData = new();
            gridPositionData.GridPosition = gridPosition;

            GridObject gridObject = instance.GetComponent<GridObject>();
            gridObject.Initialize(this, gridPosition);
            gridObject.SetLock(isLocked, lockCounter);
            gridPositionData.GridObject = gridObject;

            gridPositionData.SubgridObjects = new SubgridObject[gridObject.SubgridSize.x * gridObject.SubgridSize.y];
            gridPositionData.ID = gridPosition.ToString();

            _gridObjects.Add(gridPositionData);
            if (!isEnabled)
            {
                instance.SetActive(false);
            }
        }

        public void AddSubgridObject(Vector2Int gridPosition, Vector2Int subgridPosition, Sprite sprite, bool isTaped, bool isBird)
        {
            GridObject gridObject = GetGridObject(gridPosition);
            GameObject instance = Instantiate(_subgridObjectPrefab, gridObject.transform);
            SubgridObject subgridObject = instance.GetComponent<SubgridObject>();
            subgridObject.SetTape(isTaped);

            SpriteRenderer spriteRenderer = subgridObject.SpriteRenderer;
            spriteRenderer.sprite = sprite;
            if (subgridPosition.y > 0)
            {
                subgridObject.SetStorageLayer(false);
                subgridObject.SetShadow(true);
                instance.transform.position = gridObject.GetPositionFromSubgridPosition(subgridPosition) + Vector3.down * _backLayerOffset;
            }
            else
            {
                subgridObject.SetStorageLayer(true);
                subgridObject.SetShadow(false);
                instance.transform.position = gridObject.GetPositionFromSubgridPosition(subgridPosition) + Vector3.up * _forwardLayerOffset;
            }
            _gridObjects.FirstOrDefault(data => data.GridPosition == gridPosition);

            int index = _gridObjects.FindIndex(data => data.GridPosition == gridPosition);
            int subIndex = subgridPosition.y * gridObject.SubgridSize.x + subgridPosition.x;
            _gridObjects[index].SubgridObjects[subIndex] = subgridObject;
            if(isBird)
            {
                BirdSpawnedEvent?.Invoke(instance.transform);
            }
        }

        public GridObject GetGridObject(Vector2Int gridPosition)
        {
            GridPositionData gridPositionData = _gridObjects.FirstOrDefault(data => data.GridPosition == gridPosition);
            return gridPositionData?.GridObject;
        }

        public void MoveSubgridObject(
            Vector2Int currentGridPosition,
            Vector2Int currentSubgridPosition,
            Vector2Int newGridPosition,
            Vector2Int newSubgridPosition
            )
        {
            GridObject currentGridObject = GetGridObject(currentGridPosition);

            int currentIndex = _gridObjects.FindIndex(data => data.GridPosition == currentGridPosition);
            int currentSubIndex = currentSubgridPosition.y * currentGridObject.SubgridSize.x + currentSubgridPosition.x;

            GridObject newGridObject = GetGridObject(newGridPosition);

            int newIndex = _gridObjects.FindIndex(data => data.GridPosition == newGridPosition);
            int newSubIndex = newSubgridPosition.y * newGridObject.SubgridSize.x + newSubgridPosition.x;

            SpriteRenderer spriteRenderer = _gridObjects[currentIndex].SubgridObjects[currentSubIndex].SpriteRenderer;

            _gridObjects[currentIndex].SubgridObjects[currentSubIndex].transform.SetParent(_gridObjects[newIndex].GridObject.transform);
            if (newSubgridPosition.y > 0)
            {
                _gridObjects[currentIndex].SubgridObjects[currentSubIndex].SetStorageLayer(false);
                if (!spriteRenderer.color.Equals(Color.black))
                {
                    _gridObjects[currentIndex].SubgridObjects[currentSubIndex].SetShadow(true);
                }
                _gridObjects[currentIndex].SubgridObjects[currentSubIndex].transform.position =
                    _gridObjects[newIndex].GridObject.GetPositionFromSubgridPosition(newSubgridPosition) + Vector3.down * 1.5f;
            }
            else
            {
                _gridObjects[currentIndex].SubgridObjects[currentSubIndex].SetStorageLayer(true);
                if (!spriteRenderer.color.Equals(Color.black))
                {
                    _gridObjects[currentIndex].SubgridObjects[currentSubIndex].SetShadow(false);
                }
                _gridObjects[currentIndex].SubgridObjects[currentSubIndex].transform.position =
                    _gridObjects[newIndex].GridObject.GetPositionFromSubgridPosition(newSubgridPosition) + Vector3.up * 0.25f;
            }

            _gridObjects[newIndex].SubgridObjects[newSubIndex] = _gridObjects[currentIndex].SubgridObjects[currentSubIndex];
            _gridObjects[currentIndex].SubgridObjects[currentSubIndex] = null;
        }

        public void DestroySubgridObject(Vector2Int gridPosition, Vector2Int subgridPosition, bool isBird)
        {
            int index = _gridObjects.FindIndex(data => data.GridPosition == gridPosition);
            GridObject gridObject = GetGridObject(gridPosition);

            int subIndex = subgridPosition.y * gridObject.SubgridSize.x + subgridPosition.x;

            var subgridObject = _gridObjects[index].SubgridObjects[subIndex].gameObject;

            if (!isBird)
            {
                MergeObjectEvent?.Invoke(subgridObject.transform.position);
            }
            else
            {
                BirdMergedEvent?.Invoke(subgridObject.transform.position);
            }
            Destroy(subgridObject);

            _gridObjects[index].SubgridObjects[subIndex] = null;
        }

        public void PickSubgridObject(Vector2Int gridPosition, Vector2Int subgridPosition)
        {
            if (!_isObjectSelected)
            {
                int gridObjectIndex = _gridObjects.FindIndex(data => data.GridPosition == gridPosition);
                if (gridObjectIndex < 0 || gridObjectIndex >= _gridObjects.Count)
                {
                    Debugging.Log($"GridController: PickSubgridObject -> gridObjectIndex = {gridObjectIndex}. _gridObjects.Count {_gridObjects.Count}", LogStyle.Error);
                    return;
                }

                if (subgridPosition.x < 0 || subgridPosition.x >= _gridObjects[gridObjectIndex].SubgridObjects.Length)
                {
                    Debugging.Log($"GridController: PickSubgridObject -> subgridPosition.x = {subgridPosition.x}. _gridObjects[gridObjectIndex].Length {_gridObjects.Count}", LogStyle.Error);
                    return;
                }
                SubgridObject subgridObject = _gridObjects[gridObjectIndex].SubgridObjects[subgridPosition.x];
                if (subgridObject != null && !_levelStorage.CheckIsObjectLocked(gridPosition, subgridPosition, false))
                {
                    _objectDragger.SelectObject(subgridObject.gameObject);
                    _selectedGridPosition = gridPosition;
                    _selectedSubgridPosition = subgridPosition;
                    _selectedSubgridPosition.y = 0;
                    subgridObject.SetLayer(_draggedObjectLayer);
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameGrabObject);
                    _isObjectSelected = true;
                }
            }
        }

        public void ReleaseSubgridObject()
        {
            _objectDragger.DeselectObject(_hoveredGridObject, _isHoverOverPerson);
            _isObjectSelected = false;
        }

        public void CancelMoving(GameObject subgridObject)
        {
            ReturnSubgridObject(subgridObject);
        }

        public void ReturnSubgridObject(GameObject subgridObject)
        {
            int gridObjectIndex = _gridObjects.FindIndex(data => data.GridPosition == _selectedGridPosition);
                subgridObject.transform.position = _gridObjects[gridObjectIndex].GridObject.GetPositionFromSubgridPosition(_selectedSubgridPosition) 
                    + Vector3.up * _forwardLayerOffset;
            subgridObject.GetComponent<SubgridObject>().SetLayer(_defaultSubgridObjectLayer);
            _objectDragger.ReturnDraggedObject();
            _isObjectSelected = false;
        }

        public void PlaceSubgridObject(GameObject subgridObject)
        {
            Vector2Int newGridPosition = _hoveredGridObject.GridPosition;
            int newIndex = _gridObjects.FindIndex(data => data.GridPosition == newGridPosition);
            Vector2Int newSubgridPosition = _gridObjects[newIndex].GridObject.GetSubgridPosition(subgridObject.transform.position);
            newSubgridPosition.y = 0;
            int newSubIndex = newSubgridPosition.x;

            if (_levelStorage.CheckContainerContent(newGridPosition, newSubgridPosition) == ObjectType.None && !_levelStorage.CheckIsObjectLocked(newGridPosition, newSubgridPosition, true))
            {
                MoveSubgridObject(_selectedGridPosition, _selectedSubgridPosition, newGridPosition, newSubgridPosition);

                ObjectType objectType = _levelStorage.CheckContainerContent(_selectedGridPosition, _selectedSubgridPosition);
                _levelStorage.MoveContainerObject(_selectedGridPosition, _selectedSubgridPosition, newGridPosition, newSubgridPosition);
                _levelStorage.CheckObjectsInFirstRow(newGridPosition);
                _isObjectSelected = false;
            }
            else
            {
                ReturnSubgridObject(subgridObject);
            }
            AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGamePutObject);
            subgridObject.GetComponent<SubgridObject>().SetLayer(_defaultSubgridObjectLayer);
        }

        public void FeedSubgridObject(GameObject subgridObject)
        {
            if (_levelStorage.CheckHungryPersonObject(_selectedGridPosition, _selectedSubgridPosition))
            {
                DestroySubgridObject(_selectedGridPosition, _selectedSubgridPosition, false);
                _levelStorage.RemovePersonObject(_selectedGridPosition, _selectedSubgridPosition);
                _isObjectSelected = false;
            }
            else
            {
                ReturnSubgridObject(subgridObject);
            }
        }

        public void PackSubgridObject(Vector2Int gridPosition, Vector2Int subgridPosition)
        {
            int gridObjectIndex = _gridObjects.FindIndex(data => data.GridPosition == gridPosition);
            GridObject gridObject = GetGridObject(gridPosition);
            int subgridObjectIndex = subgridPosition.y * gridObject.SubgridSize.x + subgridPosition.x;
            _gridObjects[gridObjectIndex].SubgridObjects[subgridObjectIndex].GetComponent<SubgridObject>().SetPackage(true);
        }

        public void UnpackSubgridObject(Vector2Int gridPosition, Vector2Int subgridPosition)
        {
            int gridObjectIndex = _gridObjects.FindIndex(data => data.GridPosition == gridPosition);
            GridObject gridObject = GetGridObject(gridPosition);
            int subgridObjectIndex = subgridPosition.y * gridObject.SubgridSize.x + subgridPosition.x;
            _gridObjects[gridObjectIndex].SubgridObjects[subgridObjectIndex].GetComponent<SubgridObject>().SetPackage(false);
            UnpackObjectEvent?.Invoke(_gridObjects[gridObjectIndex].SubgridObjects[subgridObjectIndex].transform.position);
        }

        public void HideSubgridObject(Vector2Int gridPosition, Vector2Int subgridPosition)
        {
            int gridObjectIndex = _gridObjects.FindIndex(data => data.GridPosition == gridPosition);
            GridObject gridObject = GetGridObject(gridPosition);
            int subgridObjectIndex = subgridPosition.y * gridObject.SubgridSize.x + subgridPosition.x;
            _gridObjects[gridObjectIndex].SubgridObjects[subgridObjectIndex].Hide();
        }

        public void ChangeSubgridObjectSprite(Vector2Int gridPosition, Vector2Int subgridPosition, Sprite sprite)
        {
            int gridObjectIndex = _gridObjects.FindIndex(data => data.GridPosition == gridPosition);
            GridObject gridObject = GetGridObject(gridPosition);
            int subgridObjectIndex = subgridPosition.y * gridObject.SubgridSize.x + subgridPosition.x;
            _gridObjects[gridObjectIndex].SubgridObjects[subgridObjectIndex].SpriteRenderer.sprite = sprite;
        }

        public Vector3 GetObjectPosition(Vector2Int gridPosition, Vector2Int subgridPosition)
        {
            return GetGridObject(gridPosition).GetPositionFromSubgridPosition(subgridPosition);
        }

        [Serializable]
        public class GridPositionData
        {
            public string ID;
            public Vector2Int GridPosition;
            public GridObject GridObject;
            public SubgridObject[] SubgridObjects;
        }
    }
}
