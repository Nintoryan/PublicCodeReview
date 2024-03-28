using System;

namespace VillageGame.UI.Buttons
{
    public class AddBuildingButton : UIButton
    {
        private Action _callback;

        protected override void OnClick()
        {
            _callback?.Invoke();
        }

        public void SetData(Action callback)
        {
            _callback = callback;
        }
    }
}
