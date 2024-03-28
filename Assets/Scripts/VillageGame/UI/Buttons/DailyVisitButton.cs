using System;
using System.Collections.Generic;
using DG.Tweening;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace VillageGame.UI.Buttons
{
    public class DailyVisitButton : UIButton
    {
        [SerializeField] private List<MPImage> _backgrounds;
        [SerializeField] private BoxStyle _receivedStyle;
        [SerializeField] private BoxStyle _readyStyle;
        [SerializeField] private BoxStyle _notReadyStyle;

        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _exclamationMark;
        [SerializeField] private TMP_Text _dayNumberText;
        [SerializeField] private GameObject _amountToShow;
        [SerializeField] private TMP_Text _amount;
        
        public int DayNumber { get; private set; }
        

        public void Init(int dayNumber, Sprite rewardImage, int amountToShow = -1)
        {
            _icon.sprite = rewardImage;
            DayNumber = dayNumber;
            _amountToShow.SetActive(amountToShow != -1);
            _amount.text = $"х{amountToShow}";
            _dayNumberText.SetText($"День {dayNumber+1} ");
        }

        public void SetReceived(float duration = -1f)
        {
            foreach (var background in _backgrounds) 
                _receivedStyle.ApplyToMpImage(background,duration);
            
            _exclamationMark.SetActive(false);

            if (Math.Abs(duration + 1) < 0.1f)
            {
                _dayNumberText.color = DodoColors.TextWhite;    
            }
            else
            {
                _dayNumberText.DOColor(DodoColors.TextWhite, duration);
            }
        }

        public void SetReady()
        {
            foreach (var background in _backgrounds) 
                _readyStyle.ApplyToMpImage(background);
            
            _exclamationMark.SetActive(true);
            _dayNumberText.color = DodoColors.TextPumpkin;
        }

        public void SetNotReady()
        {
            foreach (var background in _backgrounds) 
                _notReadyStyle.ApplyToMpImage(background);
            
            _exclamationMark.SetActive(false);
            _dayNumberText.color = DodoColors.TextOtherGray;
        }

        protected override void OnClick()
        {
            //
        }
    }
}