using System;
using UnityEngine;

namespace VillageGame.UI.Indicators
{
    public class NewYearProjectTimerUIIndicator : UIIndicator
    {
        [SerializeField] private FillArea _fillArea;

        public void SetTime(int value, int maxValue)
        {
            UpdateValue(TimeSpan.FromSeconds(maxValue - value).ToString());
            _fillArea.SetValue(value, maxValue);
        }
    }
}
