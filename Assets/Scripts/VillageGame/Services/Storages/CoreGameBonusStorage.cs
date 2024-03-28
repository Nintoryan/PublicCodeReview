using SO;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;

namespace VillageGame.Services.Storages
{
    public class CoreGameBonusStorage : ILoading, IInitializable
    {
        public int CurrentShuffles { get; private set; }
        public int CurrentHints { get; private set; }

        public Action<int> CoreGameShufflesUpdatedEvent;
        public Action<int> CoreGameHintsUpdatedEvent;

        [Inject] public CoreGameBonusStorage(){SubscribeToEvents(true);}
        
        [Inject]
        public void Initialize()
        {
            Debug.Log("Core Game Bunuses Initialized");
        }

        ~CoreGameBonusStorage()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                StaticPrefs.CoreGameShufflesUpdatedEvent += OnCoreGameShufflesUpdatedEvent;
                StaticPrefs.CoreGameHintsUpdatedEvent += OnCoreGameHintsUpdatedEvent;
            }
            else
            {
                StaticPrefs.CoreGameShufflesUpdatedEvent -= OnCoreGameShufflesUpdatedEvent;
                StaticPrefs.CoreGameHintsUpdatedEvent -= OnCoreGameHintsUpdatedEvent;
            }
        }

        private void OnCoreGameHintsUpdatedEvent()
        {
            CurrentHints = StaticPrefs.CoreGameHints;
            CoreGameHintsUpdatedEvent?.Invoke(CurrentHints);
            Debugging.Log($"CoreGameBonusStorage: CoreGame hints set to {CurrentHints}", ColorType.Olive);
        }

        private void OnCoreGameShufflesUpdatedEvent()
        {
            CurrentShuffles = StaticPrefs.CoreGameShuffles;
            CoreGameShufflesUpdatedEvent?.Invoke(CurrentShuffles);
            Debugging.Log($"CoreGameBonusStorage: CoreGame shuffles set to {CurrentShuffles}", ColorType.Olive);
        }

        public void AddShuffles(int value) => SetShuffles(CurrentShuffles + value);

        public void RemoveShuffles(int value) => SetShuffles(CurrentShuffles - value);

        private void SetShuffles(int value)
        {
            CurrentShuffles = value;
            StaticPrefs.CoreGameShuffles = CurrentShuffles;
            CoreGameShufflesUpdatedEvent?.Invoke(CurrentShuffles);
            Debugging.Log($"CoreGameBonusStorage: CoreGame shuffles set to {CurrentShuffles}", ColorType.Olive);
        }

        public void AddHints(int value) => SetHints(CurrentHints + value);
        public void RemoveHints(int value) => SetHints(CurrentHints - value);

        private void SetHints(int value)
        {
            CurrentHints = value;
            StaticPrefs.CoreGameHints = CurrentHints;
            CoreGameHintsUpdatedEvent?.Invoke(CurrentHints);
            Debugging.Log($"CoreGameBonusStorage: CoreGame hints set to {CurrentHints}", ColorType.Olive);
            
        } 

        public bool IsEnoughShuffles(int value) => value <= CurrentShuffles;

        public bool IsEnoughHints(int value) => value <= CurrentHints;

        public void Load(LoadData request)
        {
            SetShuffles(request.data.core_game_shuffles);
            SetHints(request.data.core_game_hints);
        }
    }
}
