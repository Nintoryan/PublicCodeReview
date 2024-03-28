using DG.Tweening;
using System;
using UnityEngine;
using VillageGame.Services;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels.Warnings
{
    public class EndStoryWarningPanel : WarningPanel
    {
        [SerializeField] private EventButton _okButton;
        [SerializeField] private GameObject _fedor;

        private void OnEnable()
        {
            _okButton.ClickEvent += OnOkButtonClickEvent;
        }

        private void OnDisable() 
        {
            _okButton.ClickEvent -= OnOkButtonClickEvent;
        }

        private void OnOkButtonClickEvent()
        {
            Hide();
            _fedor.SetActive(false);
        }

        protected override void OnShown()
        {
            _fedor.SetActive(true);
        }
    }
}
