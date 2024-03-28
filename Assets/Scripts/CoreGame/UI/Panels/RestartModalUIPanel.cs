using CoreGame.UI.Indicators;
using System;
using UnityEngine;
using VContainer;
using VillageGame.Services.Storages;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels.Warnings;

namespace CoreGame.UI.Panels
{
    public class RestartModalUIPanel : ModalUIPanel
    {
        [SerializeField] private SceneLoader _sceneLoader;

        [SerializeField] private EnergyUIIndicator _energyUIIndicator;
        [SerializeField] private EventButton _restartButton;
        [SerializeField] private EventButton _returnToVillageButton;
        [SerializeField] private EnergyWarningUIPanel _energyWarningUIPanel;

        private EnergyStorage _energyStorage;

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _energyStorage = objectResolver.Resolve<EnergyStorage>();
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _energyUIIndicator.UpdateValue(_energyStorage.CurrentValue.ToString());
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _restartButton.ClickEvent += OnRestartButtonClickEvent;
                _returnToVillageButton.ClickEvent += OnReturnToVillageButtonClickEvent;
            }
            else
            {
                _restartButton.ClickEvent -= OnRestartButtonClickEvent;
                _returnToVillageButton.ClickEvent -= OnReturnToVillageButtonClickEvent;
            }
        }

        private void OnRestartButtonClickEvent()
        {
            if (_energyStorage.IsEnoughEnergy(1))
            {
                _sceneLoader.ReloadScene(false);
            }
            else
            {
                _energyWarningUIPanel.Show();
            }
        }

        private void OnReturnToVillageButtonClickEvent()
        {
            _sceneLoader.UnloadScene(false);
        }
    }
}
