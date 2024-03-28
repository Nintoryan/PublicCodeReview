using UnityEngine;

namespace CoreGame
{
    public class SubgridObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _object;
        [SerializeField] private SpriteRenderer _package;
        [SerializeField] private SpriteRenderer _tape;

        public SpriteRenderer SpriteRenderer { get => _object; set => _object = value; }

        public void SetPackage(bool isPacked)
        {
            _package.gameObject.SetActive(isPacked);
            _object.gameObject.SetActive(!isPacked);
        }

        public void SetTape(bool isTaped)
        {
            _tape.gameObject.SetActive(isTaped);
        }

        public void SetLayer(int layerId)
        {
            _object.sortingOrder = layerId;
            _package.sortingOrder = layerId + 1;
        }

        public void Hide()
        {
            _object.color = Color.black;
        }

        public void SetShadow(bool isInShadow)
        {
            _object.color = isInShadow ? Color.gray : Color.white;
            _package.color = isInShadow ? Color.gray : Color.white;
        }

        public void SetStorageLayer(bool isForward)
        {
            if (isForward)
            {
                _object.sortingOrder = 3;
                _package.sortingOrder = 4;
                _tape.sortingOrder = 4;
            }
            else
            {
                _object.sortingOrder = 2;
                _package.sortingOrder = 3;
                _tape.sortingOrder = 3;
            }
        }
    }
}
