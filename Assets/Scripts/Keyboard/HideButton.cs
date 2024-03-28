using Services;
using UIElements;
using UnityEngine;

namespace Keyboard
{
    public class HideButton : MonoBehaviour
    {
        [SerializeField] private ButtonPressObserver _button;
        [SerializeField] private ImageColorToggle _colorToggle;
        [SerializeField] private UIKeyboard _keyboard;

        private void Start()
        {
            _button.OnDown += OnDown;
            _button.OnUp += OnUp;
        }

        private void OnDestroy()
        {
            _button.OnDown -= OnDown;
            _button.OnUp -= OnUp;
        }

        private void OnUp(float obj)
        {
            _colorToggle.SetDisableColor();
        }

        private void OnDown()
        {
          //  _keyboard.Hide();
            _colorToggle.SetEnableColor();
        }

        private void OnValidate()
        {
            _button = GetComponentInChildren<ButtonPressObserver>();
            _colorToggle = GetComponentInChildren<ImageColorToggle>();
        }
    }
}