using System;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Data;
using VillageGame.UI.Elements;
using Web.Api;
using Web.ResponseStructs;

namespace VillageGame.UI.Buttons
{
    public class DailyTaskButton: UIButton
    {
        [Header("General")]
        [SerializeField] private Image _iconTask;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private TextMeshProUGUI _desctiptionText;
        [SerializeField] private GameObject _completedIcon;
        
        [Header("Rewards")]
        [SerializeField] private TextPanel _rewardRating;
        [SerializeField] private TextPanel _rewardCurrency;
        [SerializeField] private TextPanel _rewardHints;
        [SerializeField] private TextPanel _rewardShuffles;
        [SerializeField] private GameObject _rewardDecoration;
        [SerializeField] private Image _decorationIcon;
        
        [Header("Receive Button")]
        [SerializeField] private MPImage _button;
        [SerializeField] private BoxStyle _notReadyStyle;
        [SerializeField] private BoxStyle _readyStyle;
        [SerializeField] private BoxStyle _doneStyle;

        [Header("Fill")]
        [SerializeField] private MPImage _fill;
        [SerializeField] private float _fillMin;
        [SerializeField] private float _fillMax;
        [SerializeField] private TMP_Text _steps;
        
        
        public Action<DailyTaskButton> PressEvent;
        public TaskState State { get; private set; }
        public ConditionData Condition { get; private set; }

   
        protected override void OnClick()
        {
            PressEvent?.Invoke(this);
        }

        public void SetCondition(ConditionData conditionData)
        {
            Condition = conditionData;
        }

        public void SetIcon(Sprite sprite)
        {
            _iconTask.sprite = sprite;
        }

        public void SetDescription(string text)
        {
            _desctiptionText.SetText(text);
        }

        #region SetRewards

        public void SetRatingReward(int value) => SetReward(value, _rewardRating);
        public void SetCurrencyReward(int value) => SetReward(value,_rewardCurrency);
        public void SetHintsReward(int value) => SetReward(value,_rewardHints);
        public void SetShufflesReward(int value) => SetReward(value,_rewardShuffles);

        public void SetDecoration(int value, Sprite sprite = null)
        {
            if (value == -1)
            {
                _rewardDecoration.gameObject.SetActive(false);
            }
            else
            {
                _decorationIcon.sprite = sprite;
            }
        }

        private void SetReward(int value, TextPanel panel)
        {
            if (value <= 0)
            {
                panel.gameObject.SetActive(false);
            }
            else
            {
                panel.gameObject.SetActive(true);
                panel.SetText(value.ToString());    
            }
        }

        #endregion

        public void SetInProgressMode(int current, int max)
        {
            State = TaskState.in_progress;
            _completedIcon.SetActive(false);

            _buttonText.text = "Забрать награду";
            _buttonText.color = DodoColors.TextDisabledButton;

            if (max <= -1)
            {
                max = 1;
                current = 0;
            }

            if (current > max)
            {
                current = max;
                SetCompleteMode(max);
                return;
            }
            
            _steps.SetText($"{current}/{max}");

            if (current == 0)
            {
                _fill.gameObject.SetActive(false);
            }
            else
            {
                _fill.gameObject.SetActive(true);
                var sizeDelta = _fill.rectTransform.sizeDelta;
                var width = _fillMax * current / max;
                width = Mathf.Clamp(width, _fillMin, _fillMax);
                sizeDelta = new Vector2(width, sizeDelta.y);
            
                _fill.rectTransform.sizeDelta = sizeDelta;
            }
            
            
            
            _notReadyStyle.ApplyToMpImage(_button);
        }

        public void SetCompleteMode(int max)
        {
            State = TaskState.complited;

            if (max <= -1)
            {
                max = 1;
            }
            
            _buttonText.text = "Забрать награду";
            _buttonText.color = DodoColors.TextWhite;

            _steps.SetText($"{max}/{max}");
            
            _fill.gameObject.SetActive(true);
            _completedIcon.SetActive(true);
            
            var sizeDelta = _fill.rectTransform.sizeDelta;
            
            sizeDelta = new Vector2(_fillMax, sizeDelta.y);
            
            _fill.rectTransform.sizeDelta = sizeDelta;
            
            _readyStyle.ApplyToMpImage(_button);
        }

        public void SetCloseMode(int max)
        {
            State = TaskState.close;
            
            _fill.gameObject.SetActive(true);
            
            _buttonText.text = "Награда получена!";
            _buttonText.color = DodoColors.TextWhite;
            
            _steps.SetText($"{max}/{max}");
            
            button.TryGetComponent(out Image image);
            var buttonColor = image.color;
            buttonColor.a = 100;
            image.color = buttonColor;
            button.interactable = false;
            _completedIcon.SetActive(false);
            
            var sizeDelta = _fill.rectTransform.sizeDelta;
            
            sizeDelta = new Vector2(_fillMax, sizeDelta.y);
            
            _fill.rectTransform.sizeDelta = sizeDelta;
            
            _doneStyle.ApplyToMpImage(_button);
        }

        public void SetDeepLinkMode(int max, string link)
        {
            State = TaskState.in_progress;
            
            _fill.gameObject.SetActive(true);
            
            _buttonText.text = "Заказать";
            _buttonText.color = DodoColors.TextWhite;
            
            _steps.SetText($"{0}/{max}");

            button.onClick.AddListener(() => JSAPI.SendURL(link)); 
            button.TryGetComponent(out Image image);
            var buttonColor = image.color;
            buttonColor.a = 100;
            image.color = buttonColor;
            button.interactable = true;
            _completedIcon.SetActive(false);
            
            var sizeDelta = _fill.rectTransform.sizeDelta;
            
            sizeDelta = new Vector2(_fillMax, sizeDelta.y);
            
            _fill.rectTransform.sizeDelta = sizeDelta;
            
            _readyStyle.ApplyToMpImage(_button);
        }
    }
}