using UnityEngine;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class WarningPanel : UIPanel
    {
        [SerializeField] private EventButton _closeButton;

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        protected virtual void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                if(_closeButton != null)
                    _closeButton.ClickEvent += OnClickEvent;
            }
            else
            {
                if(_closeButton != null)
                    _closeButton.ClickEvent -= OnClickEvent;
            }
        }

  

        private void OnClickEvent()
        {
            Hide();
        }
    }
}