using UnityEngine;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class NoDecorationSpaceWarningPanel : WarningPanel
    {
        [SerializeField] private EventButton _okButton;
        protected override void SubscribeToEvents(bool flag)
        {
            base.SubscribeToEvents(flag);
            if (flag)
            {
                _okButton.ClickEvent += ClickEvent;
            }
            else
            {
                _okButton.ClickEvent -= ClickEvent;
            }
        }

        private void ClickEvent()
        {
            Hide();
        }
    }
}