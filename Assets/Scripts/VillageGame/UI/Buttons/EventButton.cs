using System;
using Util;

namespace VillageGame.UI.Buttons
{
    public class EventButton : UIButton
    {
        public event Action ClickEvent;
        protected override void OnClick()
        {
            Debugging.Log($"{gameObject.name}  click");
            ClickEvent?.Invoke();
        }
    }
}