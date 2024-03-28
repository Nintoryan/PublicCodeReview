using System;
using UnityEngine;
using UnityEngine.AI;
using VillageGame.Logic.Tree;
using VillageGame.Services.Characters;
using Random = UnityEngine.Random;

namespace VillageGame.Logic.Characters
{
    public class CharacterMove : MonoBehaviour
    {
        [SerializeField] private Character _owner;
        [SerializeField] private NavMeshAgent _agent;

        private CharactersNavigationService _navigationService;
        private ChristmasTree _christmasTree;

        public Vector3 LookTarget { get; set; }

        private float _idleTimer;
        private bool _isBlock;

        public void Init(CharactersNavigationService navigationService, ChristmasTree christmasTree)
        {
            _navigationService = navigationService;
            _christmasTree = christmasTree;
            SubscribeToEvents(true);
            SetNextTarget();
        }
        
        private void SubscribeToEvents(bool flag)
        {
            if (_christmasTree == null) return;
            
            if (flag)
            {
                _christmasTree.FullEvent += OnChristmasTreeFullEvent;
                _christmasTree.EmptyEvent += OnChristmasTreeEmptyEvent;
            }
            else 
            {
                _christmasTree.FullEvent -= OnChristmasTreeFullEvent;
                _christmasTree.EmptyEvent -= OnChristmasTreeEmptyEvent;
            }
        }
        
        private void OnDestroy()
        {
            SubscribeToEvents(false);
        }

        private void OnEnable()
        {
            if (_isBlock)
            {
                return;
            }
            if (!_agent.hasPath && _navigationService!= null)
            {
                SetNextTarget();
            }
        }
        
        private void Update()
        {
            if (!_agent.enabled ) return;

            if (_agent.velocity.magnitude > 0.1f)
            {
                LookTarget = transform.position + _agent.velocity;
                return;
            }

            if (_christmasTree.IsFilled())
            {
                LookTarget = _christmasTree.transform.position;
                return;
            }

            _idleTimer -= Time.deltaTime;
            
            if (_idleTimer < 0)
            {
                _idleTimer = 2f;
                SetNextTarget();
            }
        }

        private void SetNextTarget()
        {
            if (_christmasTree.IsFilled())
            {
                SetDestination(_navigationService.CaptureChristmasTreePoint(_owner));
            }
            else
            {
                var random = Convert.ToInt32(Random.Range(0, 2));
                SetDestination(random == 0
                    ? _navigationService.GetRandomPoint()
                    : _navigationService.GetHomePoint(_owner.BuildingId));
            }
        }

        private void OnChristmasTreeEmptyEvent()
        {
            _navigationService.FreeTreePoint(_owner);
            SetDestination(_navigationService.GetRandomPoint());
        }

        private void OnChristmasTreeFullEvent()
        {
            SetDestination(_navigationService.CaptureChristmasTreePoint(_owner));
        }

        private void SetDestination(Vector3 position)
        {
            if (_agent.enabled && !_isBlock && gameObject.activeInHierarchy)
            {
                _agent.SetDestination(position);
            }
        }
        
        public void BlockMove()
        {
            _isBlock = true;
            _agent.enabled = false;
        }

        public void UnblockMove()
        {
            _isBlock = false;
            _agent.enabled = true;
            SetDestination(_navigationService.GetRandomPoint());
        }
    }
}