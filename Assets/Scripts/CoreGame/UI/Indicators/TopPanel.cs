using System;
using SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VillageGame.UI.Indicators;

namespace CoreGame.UI.Indicators
{
    public class TopPanel : UIIndicator
    {
        [SerializeField] private Image _fill;
        [SerializeField] private TMP_Text _levelNumber;
        [SerializeField] private GameObject _infinitySymbol;

        private void OnEnable()
        {
            SetLevelNumber(StaticPrefs.SelectedCoreGameLevel);
        }

        private void SetLevelNumber(int number)
        {
            _levelNumber.SetText($"УРОВЕНЬ <color=#FF8F3B>{number+1}</color>");
        }

        public void SetTime(int value, int maxValue)
        {
            UpdateValue(value.ToString("000"));
            _fill.fillAmount = (float)value/maxValue;
        }

        public void DeactivateTimer()
        {
            _fill.gameObject.SetActive(false);
            _infinitySymbol.SetActive(true);
            UpdateValue("");
        }
    }
}
