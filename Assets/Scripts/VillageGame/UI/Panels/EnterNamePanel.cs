using System;
using System.Linq;
using TMPro;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class EnterNamePanel : UIPanel
    {
        [SerializeField] private EnterNameOKButton _okButton;
        [SerializeField] private TextMeshProUGUI _textField;
        [SerializeField] private UIKeyboard _uiKeyboard;
        private string _defaultInput;
        public EnterNameOKButton OkButton => _okButton;
        public string Input => _textField.text;

        private void Awake()
        {
            _defaultInput = Input;
            _uiKeyboard.ChangeCharsEvent += UIKeyboardOnChangeCharsEvent;
        }

        private void OnDestroy()
        {
            _uiKeyboard.ChangeCharsEvent -= UIKeyboardOnChangeCharsEvent;
        }

        private void UIKeyboardOnChangeCharsEvent()
        {
            if (IsNameValid())
            {
                _okButton.SetEnableColor();
            }
            else
            {
                _okButton.SetDisableColor();
            }
        }

        public bool IsNameValid()
        {
            return !string.IsNullOrWhiteSpace(Input) && Input.Any(char.IsLetter) && Input != _defaultInput;
        }

        private void OnValidate()
        {
            if (_uiKeyboard == null)
            {
                _uiKeyboard = FindObjectOfType<UIKeyboard>();
            }
        }
    }
}