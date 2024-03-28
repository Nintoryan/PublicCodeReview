using SO;
using Util;
using VillageGame.UI.Indicators;

namespace CoreGame.UI.Indicators
{
    public class HintsUIIndicator : UIIndicator
    {
        private void OnEnable()
        {
            OnCoreGameHintsUpdateEvent();
            StaticPrefs.CoreGameHintsUpdatedEvent += OnCoreGameHintsUpdateEvent;
        }

        private void OnDisable()
        {
            StaticPrefs.CoreGameHintsUpdatedEvent -= OnCoreGameHintsUpdateEvent;
        }

        private void OnCoreGameHintsUpdateEvent()
        {
            UpdateValue(StaticPrefs.CoreGameHints.ToString());
        }

        public override void UpdateValue(string value)
        {
            base.UpdateValue(value);
            valueText.color = value == "0" ? DodoColors.TextPumpkin : DodoColors.TextWhite;
        }
    }
}