using System;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Data;

namespace VillageGame.UI.Buttons
{
    public class CalendarPresentButton : UIButton
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private RectTransform _buttonBody;
        [SerializeField] private Image _presentImage;
        [SerializeField] private TextMeshProUGUI _dateText;
        [SerializeField] private MPImage _buttonBackground;
        public RectTransform ButtonBody => _buttonBody;
        public PresentBoxData Data { get; private set; }
        public bool IsOpen { get; private set; }

        public Action<CalendarPresentButton> UnblockClickEvent;
        public Action<CalendarPresentButton> ReceivePresent;
        public Action<CalendarPresentButton> ShowInfo;

        private bool _isBlock;

        public void SetData(PresentBoxData presentBoxData, DayData currentDay)
        {
            Data = presentBoxData;
            Refresh(currentDay);
        }

        public void Refresh(DayData currentDay)
        {
            _title.SetText(Data.Title);
            _presentImage.sprite = Data.BoxSprite;

            if (IsOpen)
            {
                SetBackgroundButtonActive(true);
                _dateText.SetText("Посмотреть");
            }
            else
            {
                if (Data.IsCanOpen(currentDay))
                {
                    SetBackgroundButtonActive(true);
                    _dateText.SetText("Открыть");
                }
                else
                {
                    SetBackgroundButtonActive(false);

                    if (Data.OpenDate.Year == currentDay.Year &&
                        Data.OpenDate.Month == currentDay.Month &&
                        Data.OpenDate.Day - 1 == currentDay.Day)
                    {
                        _dateText.SetText("Завтра!");
                    }
                    else
                    {
                        _dateText.SetText(Data.OpenDate.GetFullYearDateText());
                    }
                }
            }
        }
        public void SetOpenedMode()
        {
            _presentImage.sprite = Data.OpenBoxSprite;
            SetBackgroundButtonActive(true);
            _dateText.SetText("Посмотреть");
            IsOpen = true;
        }

        protected override void OnClick()
        {
            UnblockClickEvent?.Invoke(this);
            if (_isBlock)
            {
                return;
            }

            if (IsOpen)
            {
                ShowInfo?.Invoke(this);
            }
            else
            {
                ReceivePresent?.Invoke(this);
            }
        }

        public void InvokeReceivePresent()
        {
            ReceivePresent?.Invoke(this);
        }

        public void BlockDefaultClickEvent(bool isBlock)
        {
            _isBlock = isBlock;
        }

        private void SetBackgroundButtonActive(bool value)
        {
            if (value)
            {
                _buttonBackground.color = DodoColors.ButtonOrange;
                _buttonBackground.OutlineColor = DodoColors.ButtonOrangeOutline;
            }
            else
            {
                _buttonBackground.color = DodoColors.ButtonGray;
                _buttonBackground.OutlineColor = DodoColors.ButtonGrayOutline;
            }
        }

   
    }
}