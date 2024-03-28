using System;
using TMPro;
using UnityEngine;

namespace VillageGame.UI.Indicators
{
    public class LevelUIIndicator : UIIndicator
    {
        [SerializeField] private RectTransform _fill;
        [SerializeField] private TMP_Text _expText;

        [SerializeField] private float _minX;
        [SerializeField] private float _maxX;

        private bool _full;
        
        
        public void UpdateExp(int current, int max)
        {
            if(_full) return;
            
            _fill.anchoredPosition =
                new Vector2(Mathf.Lerp(_minX, _maxX, (float)current / max), _fill.anchoredPosition.y);
            _expText.SetText($"{current}/{max}");
        }
        
        public override void UpdateValue(string value) 
        { 
            valueText.text = value;

            try
            {
                var level = Convert.ToInt32(value);
                
                if (level < 20) return;

                UpdateExp(100, 100);
                _full = true;
                valueText.text = 20.ToString();
                _expText.transform.parent.gameObject.SetActive(false);
            }
            catch
            {
                // ignored
            }
        }
    }
}
