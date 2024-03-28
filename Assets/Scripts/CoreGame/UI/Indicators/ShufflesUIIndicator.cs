using SO;
using Util;
using VillageGame.UI.Indicators;

namespace CoreGame.UI.Indicators
{
    public class ShufflesUIIndicator : UIIndicator
    {
        private void OnEnable()
        {
            OnCoreGameShufflesUpdateEvent();
            StaticPrefs.CoreGameShufflesUpdatedEvent += OnCoreGameShufflesUpdateEvent;
        }

        private void OnDisable()
        {
            StaticPrefs.CoreGameShufflesUpdatedEvent -= OnCoreGameShufflesUpdateEvent;
        }

        private void OnCoreGameShufflesUpdateEvent()
        {
            UpdateValue(StaticPrefs.CoreGameShuffles.ToString());
        }

        public override void UpdateValue(string value)
        {
            base.UpdateValue(value);
            valueText.color = value == "0" ? DodoColors.TextPumpkin : DodoColors.TextWhite;
        }
    }
}
