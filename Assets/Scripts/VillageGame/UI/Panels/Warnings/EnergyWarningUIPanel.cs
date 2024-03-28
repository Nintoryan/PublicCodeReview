using System;
using SO;
using TMPro;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Services;
using VillageGame.Services.Storages;
using VillageGame.UI.Buttons;
using VillageGame.UI.Indicators;

namespace VillageGame.UI.Panels.Warnings
{
    public class EnergyWarningUIPanel : WarningPanel
    {
        [SerializeField] private TMP_Text _timeToRefresh;
        [SerializeField] private EventButton _secondClose;
        [SerializeField] private RemoveDodoCoinButton _removeDodoCoinButton;
        [SerializeField] private DodoCoinsUIIndicator _dodoCoinsCounter;

        private EnergyStorage _energyStorage;
        private EnergyConfig _energyConfig;

        public event Action EnergyRestoredToPlay;

        [Inject]
        private void Initialize(IObjectResolver resolver)
        {
            _energyConfig = resolver.Resolve<EnergyConfig>();
            _energyStorage = resolver.Resolve<EnergyStorage>();
        }

        private void Awake()
        {
            _secondClose.ClickEvent += HideNoAction;
        }

        protected override void OnShown()
        {
            _removeDodoCoinButton.SetData(10,$"{Extensions.GenerateRandomIdempotencyKey()}","energy",BuyEnergy);
            _dodoCoinsCounter.UpdateValue(DodoCoinService.CurrentBalance.ToString());
        }

        private void BuyEnergy()
        {
            _energyStorage.Add(1,false);
            Hide(() => EnergyRestoredToPlay?.Invoke());
            _dodoCoinsCounter.UpdateValue(DodoCoinService.CurrentBalance.ToString());
        }
        
        private void Update()
        {
            var timeLeft = _energyConfig.EnergyRecoveryTime - EnergyRestorerService.CurrentTimer;
            var minutes = timeLeft / 60;
            var seconds = timeLeft % 60;
            _timeToRefresh.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
