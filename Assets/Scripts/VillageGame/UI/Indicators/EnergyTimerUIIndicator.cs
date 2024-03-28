using SO;
using System;
using VContainer;
using VillageGame.Services;
using VillageGame.Services.Storages;

namespace VillageGame.UI.Indicators
{
    public class EnergyTimerUIIndicator : UIIndicator
    {
        private EnergyStorage _energyStorage;
        private EnergyConfig _energyConfig;
        private bool _isVisible;

        [Inject]
        private void Initialize(IObjectResolver resolver)
        {
            _energyStorage = resolver.Resolve<EnergyStorage>();
            _energyConfig = resolver.Resolve<EnergyConfig>();
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _energyStorage.EnergyRestoreStartedEvent += OnEnergyRestoreStartedEvent;
                _energyStorage.EnergyRestoreStoppedEvent += EnergyRestoreStoppedEvent;
            }
            else
            {
                _energyStorage.EnergyRestoreStartedEvent -= OnEnergyRestoreStartedEvent;
                _energyStorage.EnergyRestoreStoppedEvent -= EnergyRestoreStoppedEvent;
            }
        }

        private void OnEnergyRestoreStartedEvent()
        {
            _isVisible = true;
            Show();
        }

        private void EnergyRestoreStoppedEvent()
        {
            _isVisible = false;
            Hide();
        }

        private void Update()
        {
            if(_isVisible)
            {
                body.gameObject.SetActive(true);
                var timeLeft = _energyConfig.EnergyRecoveryTime - EnergyRestorerService.CurrentTimer;
                var minutes = timeLeft / 60;
                var seconds = timeLeft % 60;
                valueText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                body.gameObject.SetActive(false);
            }
        }
    }
}
