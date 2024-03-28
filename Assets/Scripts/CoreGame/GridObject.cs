using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreGame
{
    public class GridObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Grid _subgrid;
        [SerializeField] private GameObject _lock;
        [SerializeField] private TextMeshPro _lockCounter;

        [SerializeField] private GridController _gridController;
        [SerializeField] private Vector2Int _gridPosition;
        [SerializeField] private Vector2Int _subgridSize;

        public Vector2Int GridPosition { get => _gridPosition; set => _gridPosition = value; }
        public Vector2Int SubgridSize { get => _subgridSize; set => _subgridSize = value; }

        public void Initialize(GridController gridController, Vector2Int gridPosition)
        {
            _gridController = gridController;
            _gridPosition = gridPosition;
        }

        public Vector3 GetPositionFromSubgridPosition(Vector2Int subgridPosition)
        {
            return new Vector3(
                _subgrid.CellToWorld((Vector3Int)subgridPosition).x + _subgrid.cellSize.x / 2,
                _subgrid.CellToWorld((Vector3Int)subgridPosition).y,
                0
            );
        }

        public Vector2Int GetSubgridPosition (Vector3 position)
        {
            return (Vector2Int)_subgrid.WorldToCell(position);
        }

        public void SetLock(bool isLocked, int lockCounter)
        {
            if (isLocked)
            {
                _lockCounter.text = lockCounter.ToString();
            }
            _lock.SetActive(isLocked);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 pointerPosition = new Vector3(Camera.main.ScreenToWorldPoint(eventData.position).x, Camera.main.ScreenToWorldPoint(eventData.position).y, 0);
            Vector2Int subgridPosition = GetSubgridPosition(pointerPosition);
            _gridController.PickSubgridObject(_gridPosition, subgridPosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _gridController.ReleaseSubgridObject();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _gridController.HoveredGridObject = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _gridController.HoveredGridObject = null;
        }
    }
}
