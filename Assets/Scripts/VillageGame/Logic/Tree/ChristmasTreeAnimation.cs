using System;
using UnityEngine;

namespace VillageGame.Logic.Tree
{
    public class ChristmasTreeAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private readonly int _stage = Animator.StringToHash("Stage");
        private readonly int _idle = Animator.StringToHash("Idle");
        private readonly int _transition = Animator.StringToHash("Transition");
        private readonly int _progressionState = Animator.StringToHash("ProgressionState");
        private readonly int _load_1 = Animator.StringToHash("Load_1");
        private readonly int _load_2 = Animator.StringToHash("Load_2");
        
        public void SetProgressionState(int number) => _animator.SetInteger(_progressionState, number);
        public void SetAnimationStage(int stageNumber) => _animator.SetFloat(_stage, stageNumber);

        public void PlayTransition() => _animator.SetTrigger(_transition);
        public void PlayIdle() => _animator.SetTrigger(_idle);

        public void LoadProgressionState(int state)
        {
            switch (state)
            {
                case 1:
                    _animator.SetTrigger(_load_1);
                    break;
                case 2:
                    _animator.SetTrigger(_load_2);
                    break;
            }
        }
    }
}