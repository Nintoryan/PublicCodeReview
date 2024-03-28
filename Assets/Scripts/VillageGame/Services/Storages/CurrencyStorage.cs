using System;
using System.Threading.Tasks;
using Data.Scripts.Audio;
using VContainer;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;

namespace VillageGame.Services.Storages
{
    public class CurrencyStorage : ILoading
    {
        public int Current { get; private set; }
        public Action<int> Change;
        
        [Inject] public CurrencyStorage(){}
        
        public void Add(int value)
        {
            Current += value;
            AudioManager.Instance?.PlayAudioEvent(AudioEventType.AddCurrency);
            Change?.Invoke(Current);
        }

        public void Remove(int value)
        {
            Current -= value;
            if (Current < 0)
            {
                Current = 0;
            }
            Change?.Invoke(Current);
        }

        private void Set(int value)
        {
            Current = value;
        }

        public bool IsEnoughCurrency(int value)
        {
            return value <= Current;
        }

        public void Load(LoadData request)
        {
            Set(request.data.currency);
        }
    }
}