using UnityEngine;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels.Warnings
{
    public class NoStarsPanel : WarningPanel
    {
        [SerializeField] private EventButton _playButton;
        public EventButton PlayButton => _playButton;
        
        
    }
}