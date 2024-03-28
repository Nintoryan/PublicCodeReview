using System;
using Data.Scripts.Audio;
using TMPro;
using UnityEngine;
using Util;

public class UIKeyboard : MonoBehaviour
{
    [SerializeField] private int _characterLimit = 10;
    [SerializeField] private TextMeshProUGUI _text;

    private bool _isFirstPressed;
    public event Action ChangeCharsEvent;

    private void Awake()
    {
        if (_text == null)
        {
            Debugging.Log("Text of keyboard was null");
        }
        else
        {
            _text.SetText("Введи имя");
        }
    }

    public void InvokeButtonPress(char c)
    {
        if (_text != null)
        {
            if (_text.text == "Введи имя")
            {
                _text.text = "";
                _isFirstPressed = true;
            }

            if (_characterLimit > _text.text.Length)
            {
                if (_text.text.Length == 0)
                {
                    _text.SetText(c.ToString().ToUpper());
                }
                else
                {
                    _text.SetText(_text.text + c);
                }

                ChangeCharsEvent?.Invoke();
            }
            
            AudioManager.Instance.PlayAudioEvent(AudioEventType.Tap);
        }

        Debug.Log($"Button {c} pressed! input == null {_text == null} ");
    }

    public void InvokeClear()
    {
        if (_text != null)
        {
            if (_text.text == "Введи имя")
                return;

            if (_text.text.Length != 0)
                _text.text = _text.text.Substring(0, _text.text.Length - 1);

            if (_text.text.Length == 0)
            {
                _text.SetText("Введи имя");
            }

            ChangeCharsEvent?.Invoke();
            AudioManager.Instance.PlayAudioEvent(AudioEventType.Tap);
        }

        Debug.Log($"Button clear pressed!");
    }
}