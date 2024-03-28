using System;
using UnityEngine;

namespace SO
{
    public static class StaticPrefs
    {
        public static int SelectedCoreGameLevel = -1;
        public static int LastFinishedCoreGameLevel = -1;
        public static int LastStartedCoreGameLevel = -1;
        private static int energyValue = 0;
        private static int coreGameShuffles = 0;
        private static int coreGameHints = 0;

        private static bool isDodoCombinationCompleted = false;

        public static Action EnergyUpdatedEvent;
        public static Action CoreGameShufflesUpdatedEvent;
        public static Action CoreGameHintsUpdatedEvent;

        public static int EnergyValue 
        { 
            get => energyValue;
            set
            {
                energyValue = value;
                EnergyUpdatedEvent?.Invoke();
            }
        }

        public static int CoreGameShuffles 
        { 
            get => coreGameShuffles; 
            set
            {
                coreGameShuffles = value;
                CoreGameShufflesUpdatedEvent?.Invoke();
            }
        }
        
        public static int CoreGameHints
        { 
            get => coreGameHints; 
            set
            {
                coreGameHints = value;
                CoreGameHintsUpdatedEvent?.Invoke();
            }
        }

        public static void AddHints(int value)
        {
            coreGameHints += value;
            CoreGameHintsUpdatedEvent?.Invoke();
        }

        public static bool IsDodoCombinationCompleted { get => isDodoCombinationCompleted; set => isDodoCombinationCompleted = value; }
    }
}
