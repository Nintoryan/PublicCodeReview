using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.VillageGame.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Logic.Tutorial;
using VillageGame.Services;
using VillageGame.Services.Buildings;
using VillageGame.Services.CutScenes;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Storages;
using Web.RequestStructs;
using Web.ResponseStructs;

namespace VillageGame.Logic.Tree
{
    public class ChristmasTree : MonoBehaviour, ILoading
    {
        public List<Transform> _lookPositions;
        [SerializeField] private GameObject _body;
        [SerializeField] private Button _button;
        [SerializeField] private ChristmasTreeAnimation _animation;
        [SerializeField] private ChristmasTreeAnimationEvents _animationEvents;

        private MainerService _mainerService;
        private ProgressionService _progressionService;
        private RatingStorage _ratingStorage;

        private int _ratingCapacity;

        private bool _isSubscribe;
        private bool _isInvokeFilled;

        public Action EmptyEvent;
        public Action FullEvent;
        public Action ChangeEvent;
        public ChristmasTreeAnimation Animation => _animation;
        public ChristmasTreeAnimationEvents AnimationEvents => _animationEvents;
        public TreeBank Bank { get; private set; } = new();

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _ratingStorage = resolver.Resolve<RatingStorage>();
            _progressionService = resolver.Resolve<ProgressionService>();
            _mainerService = resolver.Resolve<MainerService>();
            resolver.Resolve<CutSceneStartController>();

            SubscribeToEvents(true);
        }

        private void OnDestroy()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (_isSubscribe && flag)
            {
                _ratingStorage.ChangeLevel -= OnLevelUp;
                _mainerService.MainerTickEvent -= AddValueToBank;
                _button.onClick.RemoveAllListeners();
                GlobalEvent.LoadVillageSceneEvent += LoadVillageSceneEvent;
            }

            if (flag)
            {
                _ratingStorage.ChangeLevel += OnLevelUp;
                _mainerService.MainerTickEvent += AddValueToBank;
                _button.onClick.AddListener(OnPress);
                GlobalEvent.LoadVillageSceneEvent += LoadVillageSceneEvent;
            }
            else
            {
                _ratingStorage.ChangeLevel -= OnLevelUp;
                _mainerService.MainerTickEvent -= AddValueToBank;
                GlobalEvent.LoadVillageSceneEvent -= LoadVillageSceneEvent;
                _button.onClick.RemoveAllListeners();
            }

            _isSubscribe = flag;
        }
        
        private void LoadVillageSceneEvent()
        {
            StartCoroutine(LoadTreeAnimation());
        }

        public void ActiveSprite(bool isActive)
        {
            _body.SetActive(isActive);
        }

        private void AddValueToBank(MainerData obj)
        {
            if (Bank.rating >= _ratingCapacity)
            {
                return;
            }

            Bank.rating += obj.RatingPerCooldown;
            if (Bank.rating > _ratingCapacity)
            {
                Bank.rating = _ratingCapacity;
            }

            ChangeEvent?.Invoke();
            if (IsFilled() && !_isInvokeFilled)
            {
                _isInvokeFilled = true;
                FullEvent?.Invoke();
            }
        }

        [ContextMenu("Fill Tree Debug")]
        private void FillTreeDebug()
        {
            AddValueToBank(_ratingCapacity - Bank.rating);
        }

        public void AddValueToBank(int value)
        {
            if (Bank.rating >= _ratingCapacity)
            {
                return;
            }

            Bank.rating += value;
            if (Bank.rating > _ratingCapacity)
            {
                Bank.rating = _ratingCapacity;
            }

            ChangeEvent?.Invoke();
            if (IsFilled() && !_isInvokeFilled)
            {
                _isInvokeFilled = true;
                FullEvent?.Invoke();
            }
        }

        private void OnLevelUp(int level)
        {
            _animation.SetAnimationStage(level);
            _animation.PlayTransition();
            SetCapacity();
        }

        private IEnumerator LoadTreeAnimation()
        {
            yield return new WaitForSeconds(0.05f);
            _animation.SetAnimationStage(_ratingStorage.Level);
        }
        
        private void OnPress()
        {
            if (Bank.rating == 0)
            {
                return;
            }

            _isInvokeFilled = false;

            _ratingStorage.AddRating(Bank.rating);
            Bank.rating = 0;

            EmptyEvent?.Invoke();
        }

        public bool IsFilled()
        {
            var percent = (float)Bank.rating / _ratingCapacity;
            return percent >= 0.7f;
        }

        private void SetCapacity()
        {
            var levelData = _progressionService.GetCurrentLevelData();
            _ratingCapacity = levelData.RatingCapacity;
        }

        public void Load(LoadData request)
        {
            if (request.data.player_progress != null)
            {
                Bank = request.data.tree_bank ?? new TreeBank();
                _animation.LoadProgressionState(GetProgressionState(request.data.village_tutorials));
                _animation.SetAnimationStage(request.data.player_progress.level);
                _animation.PlayIdle();
            }

            SetCapacity();
        }

        private int GetProgressionState(VillageTutorialData[] tutorialData)
        {
            if (tutorialData == null)
                return 0;
            if (!tutorialData.Any(tutorial => tutorial.IsWatched))
                return 0;

            if (tutorialData.Any(tutorial => tutorial.ID >= 2 && tutorial.IsWatched))
                return 2;


            return 0;
        }

    }
}