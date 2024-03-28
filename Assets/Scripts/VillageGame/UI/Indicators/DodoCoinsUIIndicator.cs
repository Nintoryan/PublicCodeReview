using System;
using VillageGame.Services;

namespace VillageGame.UI.Indicators
{
    public class DodoCoinsUIIndicator : UIIndicator
    {
        private void Update()
        {
            UpdateValue(DodoCoinService.CurrentBalance.ToString());
        }
    }
}
