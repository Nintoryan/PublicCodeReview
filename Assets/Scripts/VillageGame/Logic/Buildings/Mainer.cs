using System;
using System.Collections;
using UnityEngine;
using VillageGame.Data;

namespace VillageGame.Logic.Buildings
{
    public class Mainer
    {
        private readonly MainerData _data;
        private float _currentCooldownMinutes;
        public Action<MainerData> TickEvent;

        private bool _isBeingUpdated;

        public Mainer(MainerData data)
        {
            _data = data;
        }

        public void SetMaxCooldown()
        {
            _currentCooldownMinutes = _data.CooldownMinutes;
        }

        public IEnumerator StartUpdateMainer()
        {
            _isBeingUpdated = true;
            while (_isBeingUpdated)
            {
                yield return new WaitForSeconds(_currentCooldownMinutes * 60);
                _currentCooldownMinutes = _data.CooldownMinutes;
                TickEvent?.Invoke(_data);
            }
        }

        public void StopUpdateMainer()
        {
            _isBeingUpdated = false;
        }

        public void Load(DateTime lastExitTime, DateTime createTime, DateTime currentTime)
        {
            if (_data.CooldownMinutes == 0)
            {
                return;
            }

            TimeSpan elapsedTimeSpan = currentTime - lastExitTime;
            var elapsedTimeMinutes = (int)elapsedTimeSpan.TotalMinutes;
            var completedCooldown = elapsedTimeMinutes / _data.CooldownMinutes;

            for (int i = 0; i < completedCooldown; i++)
            {
                var ratingEarned = _data.RatingPerCooldown;
                TickEvent?.Invoke(_data);
            }

            TimeSpan elapsedCooldownTimeSpan = currentTime - createTime + TimeSpan.FromMinutes(_data.CooldownMinutes);
            var elapsedCooldownTimeMinutes = (int)elapsedCooldownTimeSpan.TotalMinutes;
            var remainingCooldownMinutes = _data.CooldownMinutes - (elapsedCooldownTimeMinutes % _data.CooldownMinutes);
            _currentCooldownMinutes = remainingCooldownMinutes;
        }
    }
}