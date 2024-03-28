using Services;
using TMPro;
using UIElements;
using UnityEngine;
using Util;

public class KeyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private ButtonPressObserver _button;
    [SerializeField] private ImageColorToggle _colorToggle;
    [SerializeField] private UIKeyboard _keyboardController;

    public char ButtonChar => _text == null ? ' ' : _text.text[0];

    private void Start()
    {
        _button.OnUp += OnUp;
        _button.OnDown += OnDown;
    }

    private void OnDestroy()
    {
        _button.OnUp -= OnUp;
        _button.OnDown -= OnDown;
    }

    private void OnDown()
    {
        _colorToggle.SetEnableColor();
        _keyboardController.InvokeButtonPress(ButtonChar);
    }

    private void OnUp(float holdTime)
    {
        _colorToggle.SetDisableColor();
    }

    public void SetInteractable(bool interactable)
    {
        _button.SetInteractable(interactable);
        if (!interactable)
        {
            _colorToggle.SetDisableColor();
        }
    }


    private void OnValidate()
    {
        _colorToggle = GetComponentInChildren<ImageColorToggle>();
    }
}