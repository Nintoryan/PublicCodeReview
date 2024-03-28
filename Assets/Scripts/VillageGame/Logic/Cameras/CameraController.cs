using System;
using UnityEngine;
using VContainer;
using VillageGame.Services;

namespace VillageGame.Logic.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("Zoom")]
        [SerializeField] private float _zoomSpeed;
        [SerializeField] private float _zoomMin;
        [SerializeField] private float _zoomMax;
        
        [Header("Inertia")]
        [SerializeField] private AnimationCurve _inertiaDeceleration;

        [Header("Bonds")] 
        [SerializeField] private Vector2 _ellipseSize;
        
        private Camera _camera;
        
        private float _startInertiaTime;
        private Vector3 _startInertia;
        private Vector3 _inertia;
        private InputService _input;

        [Inject]
        private void Construct(InputService inputService)
        {
            _input = inputService;
            _camera = Camera.main;
        }
        
        private void Update()
        {
            _input.EarlyInput();
            ProcessCamera(_input.CurrentScreenPoint,_input.ZoomDelta);
            _input.LateInput();
        }
    
        private void ProcessCamera(Vector3 currentScreenPoint,float zoomDelta)
        {
            if(_input.LastWorldPoint.magnitude == 0) return;

            Move(_input.WorldPointDelta);
            Zoom(currentScreenPoint, zoomDelta);
            Inertia();
        }

        private void Move(Vector3 worldPointDelta)
        {
            if(!_input.IsMoving()) return;
            
            if(_input.LastTouchCount != _input.CurrentTouchCount) return;
            
            if(!BorderCheck(transform.position - worldPointDelta)) return;
            
            transform.position -= worldPointDelta;
            _startInertia = worldPointDelta;
            _startInertiaTime = -1;
        }

        private void Zoom(Vector3 point,float zoomDelta)
        {
            if (zoomDelta == 0) return;
        
            var startPos = _camera.ScreenToWorldPoint(point);
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - zoomDelta * _zoomSpeed, _zoomMin, _zoomMax);
            var mouseWorldPosDiff = startPos - _camera.ScreenToWorldPoint(point);

            if (BorderCheck(transform.position + mouseWorldPosDiff))
            {
                transform.position += mouseWorldPosDiff;
            }
        }

        private void Inertia()
        {
            if(_input.IsMoving()) return;
            if (Math.Abs(_startInertiaTime + 1f) < 0.0001f)
            {
                _startInertiaTime = Time.time;
            }

            if (BorderCheck(transform.position - _inertia))
            {
                transform.position -= _inertia;
            }
            _inertia = _inertiaDeceleration.Evaluate(Time.time - _startInertiaTime) * _startInertia;
        }

        private bool BorderCheck(Vector3 position)
        {
            var x = position.x;
            var y = position.y;
            
            var a = _ellipseSize.x;
            var b = _ellipseSize.y;

            var result = x * x / (a * a) + y * y / (b * b);

            return result <= 1.0f;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            const int segments = 100;
            const float angleIncrement = 360.0f / segments;

            var previousPoint = Vector3.zero;
            
            for (var i = 0; i < segments; i++)
            {
                var angle = i * angleIncrement * Mathf.Deg2Rad;
                var x = _ellipseSize.x * Mathf.Cos(angle);
                var y = _ellipseSize.y * Mathf.Sin(angle);
                var point = new Vector3(x, y,0);

                if (previousPoint != Vector3.zero)
                {
                    Gizmos.DrawLine(previousPoint, point);
                }
                previousPoint = point;
            }
        }

    }
}
