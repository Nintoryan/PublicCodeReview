using UnityEngine;

namespace CoreGame
{
    public class ObjectDragger : MonoBehaviour
    {
        [SerializeField] private LevelStorage _levelStorage;
        [SerializeField] private GridController _gridController;
        [SerializeField] private LevelTimer _levelTimer;

        [SerializeField] private GameObject _draggedObject;
        [SerializeField] private bool _isInputLocked;

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
                _levelTimer.TimerStartedEvent += OnTimerStartedEvent;
                _levelTimer.TimerEndedEvent += OnTimerEndedEvent;
            }
            else
            {
                _levelTimer.TimerStartedEvent += OnTimerStartedEvent;
                _levelTimer.TimerEndedEvent -= OnTimerEndedEvent;
            }
        }

        private void Update()
        {
            if (!_isInputLocked)
            {
                if (_draggedObject != null)
                {
                    if (Input.touchCount <= 1)
                    {
                        _draggedObject.transform.position = new Vector3(
                            Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                            Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f,
                            0
                            );
                    }
                    else
                    {
                        _gridController.CancelMoving(_draggedObject);
                    }
                }
            }
        }

        private void OnTimerStartedEvent()
        {
            _isInputLocked = false;
        }

        private void OnTimerEndedEvent()
        {
            if (_draggedObject != null)
            {
                _gridController.ReturnSubgridObject(_draggedObject);
            }
            _isInputLocked = true;
        }

        public void SelectObject(GameObject subgridObject)
        {
            if (!_isInputLocked)
            {
                _draggedObject = subgridObject;
            }
        }

        public void DeselectObject(GridObject hoveredObject, bool isHoverOverPerson)
        {
            if (!_isInputLocked)
            {
                if (_draggedObject != null)
                {
                    if (hoveredObject == null)
                    {
                        if (isHoverOverPerson)
                        {
                            _gridController.FeedSubgridObject(_draggedObject);
                        }
                        else
                        {
                            _gridController.ReturnSubgridObject(_draggedObject);
                        }
                    }
                    else
                    {
                        _gridController.PlaceSubgridObject(_draggedObject);
                    }
                    _draggedObject = null;
                }
            }
        }

        public void ReturnDraggedObject()
        {
            _draggedObject = null;
        }
    }
}
