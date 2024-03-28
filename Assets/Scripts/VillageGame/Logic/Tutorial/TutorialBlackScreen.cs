using UnityEngine;
using UnityEngine.UI;
using VillageGame.UI.Buttons;

namespace VillageGame.Logic.Tutorial
{
    public class TutorialBlackScreen : EventButton
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _zero;
        [SerializeField] private Color _full;
        [SerializeField] private float _speed;

        public  bool IsActive { get; private set; }

        private void Update()
        {
            if (!IsActive)
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
            IsActive = isActive;
            _image.raycastTarget = isActive;
        }
    }
}