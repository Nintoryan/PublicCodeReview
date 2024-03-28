using Data.Scripts.Audio;
using SO;
using UnityEngine;

namespace CoreGame.UI.Panels
{
    public class DefeatUIPanel : EndGameUIPanel
    {
        [SerializeField] private GameObject _restartLevelUIButton;

        protected override void OnEnergyUpdateEvent()
        {
            base.OnEnergyUpdateEvent();
        }

        protected override void OnShown()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameDefeat);
        }
    }
}
