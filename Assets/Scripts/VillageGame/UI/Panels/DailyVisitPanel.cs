using System;
using Data.Scripts.Audio;
using UnityEngine;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class DailyVisitPanel : UIPanel
    {
        [SerializeField] private Transform contentTransform;
        [SerializeField] private EventButton _receiveButton;
        
        public Transform ContentTransform => contentTransform;
        public Action PressButtonEvent;

        private void Awake()
        {
            _receiveButton.ClickEvent += ()=>
            {
                _receiveButton.DisableButton();
                PressButtonEvent?.Invoke();
            };
        }

        public override void Hide(Action onHidden = null)
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.MenuOpen);
            base.Hide(onHidden);
        }
    }
}