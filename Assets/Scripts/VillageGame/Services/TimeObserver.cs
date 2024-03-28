using System;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data;
using VillageGame.Services.LoadingData;
using Web.Api;
using Web.RequestStructs;

namespace VillageGame.Services
{
    public class TimeObserver : ILoading, ITickable
    {
        private TimeZoneInfo _playerTimeZone;
        public DateTime CurrentPlayerTime { get; private set; }
        public DateTime CurrentServerTime { get; private set; }
        public DateTime LastExitTime { get; private set; }
        public DayData CurrentDay { get; private set; }

        public TimeSpan RemainingTime => _endTime - CurrentPlayerTime;

        private DateTime _lastSavedDateTime;
        private DateTime _endTime;
        private float _cooldown;
        
        public Action<TimeZoneInfo> SetTimeZoneEvent;
        public Action<int> SetBaseUtcOffset;
        public Action TimeSetEvent;
        public Action StartNewDayInGameEvent;

        public TimeZoneInfo PlayerTimeZone => _playerTimeZone;
        private float _serverTimeRequestCooldown;

        [Inject]
        public TimeObserver()
        {
            SubscribeToEvents(true);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                WebAPI.GetServerTimeEvent += SetCurrentTime;
            }
            else
            {
                WebAPI.GetServerTimeEvent -= SetCurrentTime;
            }
        }

        
        public void Tick()
        {
            if (CurrentDay == null)
            {
                return;
            }

            if (_serverTimeRequestCooldown >= 0)
            {
                _serverTimeRequestCooldown -= Time.deltaTime;
            }
            
            _cooldown += Time.deltaTime;
            
            if (!(_cooldown >= 1))
            {
                return;
            }
            
            CurrentServerTime += TimeSpan.FromSeconds(_cooldown);
            
            if (DateTime.Now.Subtract(CurrentServerTime).TotalSeconds is > 30 or < -30)
            {
                CurrentPlayerTime = CurrentServerTime;
                if (_serverTimeRequestCooldown < 0)
                {
                    WebAPI.GetServerTime();
                    _serverTimeRequestCooldown = 10;
                }
                
            }
            else
            {
                CurrentPlayerTime = DateTime.Now;
            }
            
            _cooldown = 0;

            if (CurrentPlayerTime.Day != CurrentDay.Day)
            {
                CurrentDay = new DayData
                {
                    Day = CurrentPlayerTime.Day,
                    Month = CurrentPlayerTime.Month,
                    Year = CurrentPlayerTime.Year
                };
                StartNewDayInGameEvent?.Invoke();
            }
        }

        public void Load(LoadData request)
        {
            if (request.data.base_utc_offset == 0)
            {
                SetBaseUtcOffset?.Invoke((int)TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds);
            }

            if (request.data.player_time_zone is null or "Local")
            {
                _playerTimeZone = TimeZoneInfo.Local;
                SetTimeZoneEvent?.Invoke(_playerTimeZone);
                Debugging.Log("Time zone is null or Local - set to Local");
            }
            else
            {
                Debugging.Log("Time zone is not null or Local");
                if (Extensions.TryParseDate(request.data.exit_time, out var exitTime))
                {
                    LastExitTime = exitTime;
                    Debugging.Log($"Exit time set:{exitTime}");
                }

                try
                {
                    _playerTimeZone = TimeZoneInfo.FindSystemTimeZoneById(request.data.player_time_zone);
                    Debugging.Log($"Player time zone found! {_playerTimeZone.Id}");
                }
                catch (Exception e)
                {
                    Debugging.Log($"Player time zone not found {e.Message}. Setting it to local.");
                    _playerTimeZone = TimeZoneInfo.Local;
                }
            }

            try
            {
                SetCurrentTime(request.time);
                SetExitTime(request);
            }
            catch (Exception e)
            {
                Debugging.Log($"Some shit happened here too : {e.Message}");
                throw;
            }
        }

        private void SetExitTime(LoadData request)
        {
            LastExitTime = Extensions.TryParseDate(request.data.exit_time, out var exitTime)
                ? exitTime
                : CurrentPlayerTime;
            Debugging.Log($"Exit time set: {LastExitTime}");
        }

        private void SetCurrentTime(DateTime serverTime)
        {
            CurrentServerTime = TimeZoneInfo.ConvertTime(serverTime, _playerTimeZone);
            CurrentPlayerTime = CurrentServerTime;
            int timeZoneOffsetMinutes = (int)_playerTimeZone.BaseUtcOffset.TotalMinutes;
            CurrentDay = new DayData()
            {
                Day = CurrentPlayerTime.Day,
                Month = CurrentPlayerTime.Month,
                Year = CurrentPlayerTime.Year
            };

            DateTime tomorrow = CurrentPlayerTime.AddDays(1);
            _endTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);

            Debugging.Log($"TimeObserver: SetCurrentTime -> " +
                          $"current day = day({CurrentDay.Day} month{CurrentDay.Month} year{CurrentDay.Year})" +
                          $"current time = {CurrentPlayerTime.Hour}:{CurrentPlayerTime.Minute} ", ColorType.Aqua);

            TimeSetEvent?.Invoke();
        }

       
    }
}