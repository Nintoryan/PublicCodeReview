using System;
using TMPro;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels.PresentInformation
{
    public class DodoCoinUI : MiniPanel
    {
        public EventButton DodoCoinButton;
        public TMP_Text DodoCoinDoneText;
        public TMP_Text Amount;
        public event Action GetDodoCoin;
    }
}