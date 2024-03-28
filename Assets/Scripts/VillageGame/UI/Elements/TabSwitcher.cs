using System.Collections.Generic;
using UnityEngine;

namespace VillageGame.UI.Elements
{
    public abstract class TabSwitcher : MonoBehaviour
    {
        [SerializeField] private RectTransform _activeHighlight;
        [SerializeField] private List<Vector2> _positions;
        [SerializeField] private float _speed;
        

        private void Update()
        {
            if(_activeHighlight == null) 
                return;
            
            var current = GetActivePosition();
            if (current >= 0 && current < _positions.Count)
            {
                _activeHighlight.anchoredPosition = Vector2.Lerp(_activeHighlight.anchoredPosition, _positions[current],
                    Time.deltaTime * _speed);
            }
        }

        protected abstract int GetActivePosition();
    }
}