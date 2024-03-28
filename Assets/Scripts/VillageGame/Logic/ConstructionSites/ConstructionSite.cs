using System;
using UnityEngine;
using Util;
using VillageGame.Data;

namespace VillageGame.Logic.Snowdrifts
{
    public class ConstructionSite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private ConstructionSiteData _data;
        public ConstructionSiteData Data => _data;
        
        public Action<ConstructionSite> RefreshConstructionSiteEvent;

        public void SetData(ConstructionSiteData data)
        {
            Debugging.Log($"Строительная площадка устанавливаем новые данные {data.State}");
            _data = data;
        }

        public void ActiveSprite(bool isActive)
        {
            _spriteRenderer.gameObject.SetActive(isActive);
        }
        public void SetCurrentState(StateType stateType)
        {
            Data.State = stateType;
            if (stateType == StateType.Highlighted)
            {
                _spriteRenderer.sortingLayerName = "UI";
                _spriteRenderer.sortingOrder = -1;
            }
            else
            {
                _spriteRenderer.sortingLayerName = "Building";
                _spriteRenderer.sortingOrder = -1;
            }
            RefreshConstructionSiteEvent?.Invoke(this);
        }
        public void SetActiveSprite(bool isActive)
        {
            _spriteRenderer.gameObject.SetActive(isActive);
        }
        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public enum StateType
        {
            None,
            Snowdrift,
            ConstructionSite,
            Highlighted
        }

        public enum PositionType
        {
            None,
            Nearby,
            Distant
        }
    }
}