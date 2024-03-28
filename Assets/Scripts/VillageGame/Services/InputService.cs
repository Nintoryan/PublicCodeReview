using System;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;

namespace VillageGame.Services
{
    public class InputService : ITickable
    {
        public Vector2 MousePosition => Input.mousePosition;
        public bool IsPressedOnePosition { get; private set; }
        public Vector3 CurrentScreenPoint { get; private set; }
        public Vector3 LastWorldPoint { get; private set; }
        public Vector3 WorldPointDelta { get; private set; }
        public int LastTouchCount { get; private set; }
        public int CurrentTouchCount { get; private set; }
        public float ZoomDelta { get; private set; }
        public float LastZoomDistance { get; private set; }

        private Vector3 _posStartTouch, _posEndTouch;

        private float _timeStartTouch, _timeEndTouch;
        private float _timeToLastTouch => _timeEndTouch - _timeStartTouch;
        public bool IsBlock { get; private set; } = true;

        public Action<Vector2> NonBlockStartTouchEvent;
        public Action<Vector2> StartTouchEvent;
        public Action<Vector2> PressedTouchEvent;
        public Action<Vector2, float> EndTouchEvent;
        private readonly Camera _camera;

        private float _pressCooldown;

        private bool _isMouseButtonDown;

        [Inject]
        public InputService()
        {
            _camera = Camera.main;
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                NonBlockStartTouchEvent?.Invoke(MousePosition);
            }

            if (IsBlock)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _pressCooldown = 0;
                _timeStartTouch = Time.time;
                _posStartTouch = _camera.ScreenToWorldPoint(MousePosition);
                _posEndTouch = Vector3.zero;
                IsPressedOnePosition = false;
                _isMouseButtonDown = true;
                StartTouchEvent?.Invoke(MousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                if (_pressCooldown < 1)
                {
                    if (_pressCooldown > 0.15f && WorldPointDelta != Vector3.zero && _isMouseButtonDown)
                    {
                        _isMouseButtonDown = false;
                    }

                    _pressCooldown += Time.deltaTime;
                }
                else if (_posEndTouch == Vector3.zero)
                {
                    _posEndTouch = _camera.ScreenToWorldPoint(MousePosition);
                    if (Vector3.Distance(_posStartTouch, _posEndTouch) < 0.5f && !IsPressedOnePosition)
                    {
                        IsPressedOnePosition = true;
                        _isMouseButtonDown = true;
                        PressedTouchEvent?.Invoke(MousePosition);
                    }
                }
                else
                {
                    return;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!_isMouseButtonDown)
                {
                    return;
                }

                _isMouseButtonDown = false;
                _timeEndTouch = Time.time;

                _posEndTouch = _camera.ScreenToWorldPoint(MousePosition);
                if (Vector3.Distance(_posStartTouch, _posEndTouch) < 0.5f)
                {
                    IsPressedOnePosition = true;
                }

                EndTouchEvent?.Invoke(MousePosition, _timeToLastTouch);
            }
        }

        public void EarlyInput()
        {
            if (IsBlock)
            {
                return;
            }

            if (Input.touchCount == 0)
            {
                CurrentScreenPoint = Input.mousePosition;
                ZoomDelta = Input.mouseScrollDelta.y;

                CurrentTouchCount = 0;
            }

            if (Input.touchCount == 1)
            {
                CurrentScreenPoint = Input.GetTouch(0).position;
                ZoomDelta = 0;

                CurrentTouchCount = 1;
            }
            else if (Input.touchCount == 2)
            {
                var t0 = (Vector3)Input.GetTouch(0).position;
                var t1 = (Vector3)Input.GetTouch(1).position;

                CurrentScreenPoint = (t0 + t1) / 2f;
                var zoomDistance = Vector3.Distance(_camera.ScreenToWorldPoint(t0), _camera.ScreenToWorldPoint(t1));
                if (LastZoomDistance == 0f)
                {
                    ZoomDelta = 0f;
                }
                else
                {
                    ZoomDelta = zoomDistance - LastZoomDistance;
                }

                CurrentTouchCount = 2;
            }

            WorldPointDelta = _camera.ScreenToWorldPoint(CurrentScreenPoint) - LastWorldPoint;
        }

        public void LateInput()
        {
            if (IsBlock)
            {
                return;
            }

            if (Input.touchCount == 0)
            {
                LastZoomDistance = 0f;
                LastWorldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);

                LastTouchCount = 0;
            }

            if (Input.touchCount == 1)
            {
                LastZoomDistance = 0f;
                LastWorldPoint = _camera.ScreenToWorldPoint(Input.GetTouch(0).position);

                LastTouchCount = 1;
            }
            else if (Input.touchCount == 2)
            {
                var t0 = (Vector3)Input.GetTouch(0).position;
                var t1 = (Vector3)Input.GetTouch(1).position;

                LastWorldPoint = _camera.ScreenToWorldPoint((t0 + t1) / 2f);
                LastZoomDistance = Vector3.Distance(_camera.ScreenToWorldPoint(t0), _camera.ScreenToWorldPoint(t1));

                LastTouchCount = 2;
            }
        }

        public bool IsMoving()
        {
            switch (Input.touchCount)
            {
                case 0:
                    if (Input.GetKey(KeyCode.Mouse0)) return true;
                    break;
                case 1:
                    if (Input.GetTouch(0).phase == TouchPhase.Moved) return true;
                    break;
                case 2:
                    if (Input.GetTouch(0).phase == TouchPhase.Moved
                        || Input.GetTouch(1).phase == TouchPhase.Moved) return true;
                    break;
            }

            return false;
        }

        public void BlockInput(bool isBlock)
        {
            IsBlock = isBlock;
            /*Debugging.Log($"InputService: ЗАБЛОКИРОВАТЬ ИНПУТ *** {isBlock}", ColorType.Red);*/
            if (isBlock)
            {
                WorldPointDelta = Vector3.zero;
                LastWorldPoint = Vector3.zero;
                _posStartTouch = Vector3.zero;
                _posEndTouch = Vector3.zero;
                _timeStartTouch = 0;
                _timeEndTouch = 0;
                CurrentTouchCount = 0;
                LastTouchCount = 0;
            }
            else
            {
                WorldPointDelta = _camera.ScreenToWorldPoint(CurrentScreenPoint) - LastWorldPoint;
                LastWorldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
}