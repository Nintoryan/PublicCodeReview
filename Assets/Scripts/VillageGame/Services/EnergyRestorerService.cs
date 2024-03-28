using System;
using VContainer;
using VContainer.Unity;
using SO;
using VillageGame.Services.Storages;
using System.Collections;

namespace VillageGame.Services
{
    public class EnergyRestorerService :  ITickable
    {
        private readonly EnergyConfig _energyConfig;
        private readonly EnergyStorage _energyStorage;
        private readonly TimeObserver _timeObserver;

        private static int _currentTimerProgress = 0;
        private IEnumerator _restoreCoroutine;

        public static int CurrentTimer => _currentTimerProgress;

        private DateTime _lastDateTime;

        [Inject]
        public EnergyRestorerService(IObjectResolver objectResolver)
        {
            _energyConfig = objectResolver.Resolve<EnergyConfig>();
            _energyStorage = objectResolver.Resolve<EnergyStorage>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            SubscribeToEvents(true);
        }

        ~EnergyRestorerService()
        {
            SubscribeToEvents(false);
        }

        public void Tick()
        {
            if (_energyStorage.LastRestoreTime == new DateTime())
            {
                return;
            }
            
            _currentTimerProgress = GetCurrentTimeProgress();
            
            if (_currentTimerProgress >= _energyConfig.EnergyRecoveryTime)
            {
                _energyStorage.Restore(1);
                _currentTimerProgress = 0;
            }
        }

        private int GetCurrentTimeProgress()
        {
            return (int)(_timeObserver.CurrentPlayerTime - _energyStorage.LastRestoreTime).TotalSeconds;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _energyStorage.TimeLoadedEvent += OnTimeLoadedEvent;
            }
            else
            {
                _energyStorage.TimeLoadedEvent -= OnTimeLoadedEvent;
            }
        }


        private void OnTimeLoadedEvent(int elapsedTime, int missingEnergy)
        {
            int restoredEnergy = 0;

            while (missingEnergy > 0 && elapsedTime > _energyConfig.EnergyRecoveryTime)
            {
                elapsedTime -= _energyConfig.EnergyRecoveryTime;
                missingEnergy--;
                restoredEnergy++;
            } 
            _currentTimerProgress = elapsedTime;

            if (restoredEnergy > 0)
            {
                _energyStorage.Restore(restoredEnergy);
            }
        }
    }
}
