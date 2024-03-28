using System;
using System.Collections;
using Data.Scripts.Audio;
using Febucci.UI;
using TMPro;
using UnityEngine;
using Util;
using VillageGame.Logic.Curtains;

namespace VillageGame.UI.Elements
{
    public class TextTypewriter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        [SerializeField] private TextAnimatorPlayer _textAnimatorPlayer;
        [SerializeField] private CloudCurtain _cloudCurtain;

        private string _fullText;

        private Coroutine _coroutine;
        public bool IsTyping { get; private set; }

        private void Awake()
        {
            _textMeshPro.SetText("");
            _textAnimatorPlayer.onCharacterVisible.AddListener((c) =>
            {
                if (c == ' ')
                {
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.TextType_Space);
                }
                else
                {
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.TextType);
                }
            });
            _textAnimatorPlayer.onTypewriterStart.AddListener(() =>
            {
                IsTyping = true;
            });
            _textAnimatorPlayer.onTextShowed.AddListener(() =>
            {
                IsTyping = false;
            });
        }

        private void OnDisable()
        {
            StopWrite();
        }

        public void Reset()
        {
            _fullText = "";
            _textMeshPro.SetText("");
        }

        public void StartWrite(string text, float delay = 0)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            _fullText = text;
            _coroutine = StartCoroutine(TypeText(delay));
        }


        public void StopWrite()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
           
            _textAnimatorPlayer.SkipTypewriter();
        }

        private IEnumerator TypeText(float delay)
        {
            yield return new WaitUntil(() => !CloudCurtain.IsAnimating);
            yield return new WaitForSeconds(delay);
            _textAnimatorPlayer.ShowText(_fullText);
        }

        private void OnValidate()
        {
            if (_cloudCurtain == null)
            {
                _cloudCurtain = FindObjectOfType<CloudCurtain>();
            }
        }
    }
}