using System;
using UnityEngine;

namespace VillageGame.Logic.Tree
{
    public class ChristmasTreeAnimationEvents : MonoBehaviour
    {
        public event Action EndPineConeAnimationEvent;
        public event Action EndGrowAnimationEvent;

        private void InvokeEndPineConeAnimation() => EndPineConeAnimationEvent?.Invoke();
        private void InvokeEndGrowAnimation() => EndGrowAnimationEvent?.Invoke();
    }
}