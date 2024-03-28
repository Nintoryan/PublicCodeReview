using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using UnityEngine.AI;
using Util;
using Random = UnityEngine.Random;

namespace VillageGame.Logic.Characters
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private CharacterMove _characterMove;
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;

        private static readonly int _moveHash = Animator.StringToHash("IsMoving");
        private static readonly int _upHash = Animator.StringToHash("IsUp");
        private static readonly int _idleSpeed = Animator.StringToHash("IdleSpeed");
        private static readonly int _pineCone = Animator.StringToHash("PineCone");
        private static readonly int _isCutScene = Animator.StringToHash("IsCutScene");

        private void OnEnable()
        {
            _animator.SetFloat(_idleSpeed, Random.Range(0.75f, 1.3f));
        }

        private void FixedUpdate()
        {
            SetMove(_agent.velocity.magnitude > 0.2f);
            SetLookUp(transform.position.y < _characterMove.LookTarget.y);
        }


        public void PlayPineCone()
        {
            Debugging.Log($"{gameObject.name} play pine cone");
            _animator.SetBool(_isCutScene, true);
            _animator.SetTrigger(_pineCone);
        }

        public void EndCutScene()
        {
            _animator.SetBool(_isCutScene, false);
        }

        private void SetLookUp(bool value) => _animator.SetBool(_upHash, value);

        private void SetMove(bool value) => _animator.SetBool(_moveHash, value);


        public void PlayCutSceneReaction(CharacterReaction characterReaction)
        {
            _animator.SetBool(_isCutScene, true);
     
            if (characterReaction == CharacterReaction.Chatting)
            {
                var random = Random.Range(0, 2);
                _animator.SetTrigger($"Chatting_{random}");
                return;
            }

            _animator.SetTrigger(characterReaction.ToString());
        }
    }
}