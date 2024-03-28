using System;
using DG.Tweening;
using UnityEngine;

namespace VillageGame.Logic.Cameras
{
    public class CutSceneCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _moveDuration = 1;
        [SerializeField] private float _zoomDuration = 1;

        private Vector3 _startPosition;
        private float _startOrthographicSize;

        public bool IsMovement { get; private set; }
        private Sequence _sequence;

        private void Awake()
        {
            _sequence = DOTween.Sequence();
        }

        public void SaveStartPosition()
        {
            _startPosition = _camera.transform.position;
            _startOrthographicSize = _camera.orthographicSize;
        }


        public void SetNewPosition(Vector2 position, float orthographicSize)
        {
            var camPosition = new Vector3(position.x, position.y, _camera.transform.position.z);
            _camera.transform.position = camPosition;
            _camera.orthographicSize = orthographicSize;
            IsMovement = false;
            _sequence?.Kill();
        }

        public void SetNewPositionWithAnimation(Vector2 position, float orthographicSize)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            IsMovement = true;
            var camPosition = new Vector3(position.x, position.y, _camera.transform.position.z);
            _sequence.Append(_camera.transform.DOMove(camPosition, _moveDuration)
                .SetEase(Ease.Linear)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable));

            _sequence.Join(_camera.DOOrthoSize(orthographicSize, _zoomDuration)
                .SetEase(Ease.Linear)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable));

            _sequence.AppendCallback(() => IsMovement = false);
        }

        public void SetStartPositionWithAnimation(Action OnCanceled = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            IsMovement = true;

            _sequence.Append(_camera.transform.DOMove(_startPosition, _moveDuration)
                .SetEase(Ease.Linear)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable));

            _sequence.Join(_camera.DOOrthoSize(_startOrthographicSize, _zoomDuration)
                .SetEase(Ease.Linear)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable));

            _sequence.AppendInterval(1);
            _sequence.AppendCallback(() => { OnCanceled?.Invoke(); });
            _sequence.AppendCallback(() => IsMovement = false);
        }
    }
}