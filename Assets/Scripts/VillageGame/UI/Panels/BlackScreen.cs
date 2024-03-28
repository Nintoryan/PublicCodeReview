using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VillageGame.UI.Services;

namespace VillageGame.UI.Panels
{
    public class BlackScreen : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _zero;
        [SerializeField] private Color _full;
        [SerializeField] private float _speed;

        private bool _isActive;
        
        private void Update()
        {

            if (UIWindowObserver.CurrentOpenedPanel is null or DialogueUIPanel && !_isActive)
            {
                _image.color = Color.Lerp(_image.color, _zero, Time.deltaTime * _speed);
            }
            else
            {
                _image.color = Color.Lerp(_image.color, _full, Time.deltaTime * _speed);
            }
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
        }
    }
}