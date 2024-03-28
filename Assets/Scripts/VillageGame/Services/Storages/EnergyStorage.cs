using System;
using VContainer;
using SO;
using Web.RequestStructs;
using Util;
using VillageGame.Services.LoadingData;
using Web.ResponseStructs;

namespace VillageGame.Services.Storages
{
    public class EnergyStorage : ILoading
    {
        private readonly EnergyConfig _energyConfig;
        private readonly TimeObserver _timeObserver;

        public int CurrentValue { get; private set; }
        private DateTime _lastRestoreTime;
        public DateTime LastRestoreTime => _lastRestoreTime;
        private int _lastRestoreTimeFailCheckInDays = 36500;

        public Action<EnergyData> EnergyUpdatedEvent;
        public Action<int> EnergySettedEvent;
        public Action<int, int> TimeLoadedEvent;
        public Action EnergyRestoreStartedEvent;
        public Action EnergyRestoreStoppedEvent;
        public Func<int> CurrentTimeRequestedEvent;

        [Inject]
        public EnergyStorage(IObjectResolver objectResolver)
        {
            _energyConfig = objectResolver.Resolve<EnergyConfig>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            Constance.MAX_ENERGY = _energyConfig.MaxEnergy;
        }

        public void Restore(int value)
        {
            Set(CurrentValue + value);
            _lastRestoreTime = _timeObserver.CurrentPlayerTime;
            EnergyUpdatedEvent?.Invoke(new EnergyData(CurrentValue, _lastRestoreTime));
        }

        public void ConsoleAdd(int value)
        {
            Set(CurrentValue + value, false);
            var energyData = new EnergyData(CurrentValue, _lastRestoreTime);
            EnergyUpdatedEvent?.Invoke(energyData);
        }

        public void Add(int value, bool notifyRestorer = true)
        {
            if (CurrentValue + value <= _energyConfig.MaxEnergy)
            {
                Set(CurrentValue + value, notifyRestorer);
                EnergyData energyData = new EnergyData(CurrentValue, _lastRestoreTime);
                EnergyUpdatedEvent?.Invoke(energyData);
            }
        }

        public void Remove(int value)
        {
            if (CurrentValue > 0)
            {
                if (CurrentValue == _energyConfig.MaxEnergy 
                    || _lastRestoreTime == new DateTime() 
                    || (_timeObserver.CurrentPlayerTime - _lastRestoreTime).Days > _lastRestoreTimeFailCheckInDays)
                {
                    _lastRestoreTime = _timeObserver.CurrentPlayerTime;
                }

                Set(CurrentValue - value);
                EnergyUpdatedEvent?.Invoke(new EnergyData(CurrentValue, _lastRestoreTime));
            }
        }

        private void Set(int value, bool notifyRestorer = true)
        {
            if (value < _energyConfig.MaxEnergy)
            {
                if (notifyRestorer)
                    EnergyRestoreStartedEvent?.Invoke();
           
                CurrentValue = value;
            }
            else
            {
                if (notifyRestorer)
                    EnergyRestoreStoppedEvent?.Invoke();

                CurrentValue = _energyConfig.MaxEnergy;
            }

            StaticPrefs.EnergyValue = CurrentValue;
            EnergySettedEvent?.Invoke(CurrentValue);
        }

        public bool IsEnoughEnergy(int value)
        {
            return value <= CurrentValue;
        }

        public void Load(LoadData request)
        {
            Set(request.data.energy_data.energy);
            if (request.data.energy_data.time == new DateTime())
            {
                if (CurrentValue < _energyConfig.MaxEnergy)
                {
                    _lastRestoreTime = _timeObserver.CurrentPlayerTime;
                }
                else
                {
                    _lastRestoreTime = request.data.energy_data.time;
                }
            }
            else
            {
                if ((_timeObserver.CurrentPlayerTime - request.data.energy_data.time).Days >
                    _lastRestoreTimeFailCheckInDays)
                {
                    _lastRestoreTime = _timeObserver.CurrentPlayerTime;
                }
                else
                {
                    if (CurrentValue < _energyConfig.MaxEnergy)
                    {
                        _lastRestoreTime = request.data.energy_data.time;
                        
                        TimeLoadedEvent?.Invoke(
                            (int)(_timeObserver.CurrentPlayerTime - TimeZoneInfo.ConvertTime(request.data.energy_data.time, _timeObserver.PlayerTimeZone)).TotalSeconds,
                            _energyConfig.MaxEnergy - CurrentValue);
                    }
                }
            }
        }
    }
}