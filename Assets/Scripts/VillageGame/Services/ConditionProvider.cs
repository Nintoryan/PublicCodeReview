using System;
using System.Collections;
using System.Linq;
using CoreGame;
using DefaultNamespace.VillageGame.Infrastructure;
using SO.Data;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Services.CutScenes;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Services;
using Web.ResponseStructs;
using IInitializable = VContainer.Unity.IInitializable;

namespace VillageGame.Services
{
    public class ConditionProvider : IInitializable
    {
        private readonly CoreGameLoadService _coreGameLoad;
        private readonly CharactersStorage _characterStorage;
        private readonly RatingStorage _ratingStorage;
        private readonly ConstructionSiteService _constructionSiteService;
        private readonly TimeObserver _timeObserver;

        private readonly WinCounter _winCounter;
        private readonly BuildingOnMapStorage _buildingOnMapStorage;

        private readonly CoroutineRunner _coroutineRunner;
        private readonly IceCubesService _iceCubesService;

        public Action<ConditionData> CreateNewConditionEvent;

        [Inject]
        public ConditionProvider(IObjectResolver objectResolver)
        {
            _characterStorage = objectResolver.Resolve<CharactersStorage>();
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _coreGameLoad = objectResolver.Resolve<CoreGameLoadService>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _winCounter = objectResolver.Resolve<WinCounter>();
            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();
            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }
        ~ConditionProvider()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                GlobalEvent.LoadGameEvent += LoadSceneEvent;
                _characterStorage.AddCharacter += OnAddCharacter;
                _buildingOnMapStorage.BuildBuildingEvent += OnBuildHouse;
                _ratingStorage.ChangeLevel += OnChangeLevel;
                _coreGameLoad.CoreGameWinEvent += OnCoreGameWin;
                _winCounter.NumberOfWinCountedEvent += OnNumberOfWinCounted;
                _constructionSiteService.RefreshConstructionSitesEvent += OnSnowdriftCleared;
                UIWindowObserver.SwitchWindowEvent += SwitchWindowEvent;
                CutSceneStateObserver.EndCutsceneEvent += OnEndCutScene;
                _iceCubesService.IceCubeRemovedEvent += OnIceCubeRemovedEvent;
            }
            else
            {
                GlobalEvent.LoadGameEvent -= LoadSceneEvent;
                _characterStorage.AddCharacter -= OnAddCharacter;
                _buildingOnMapStorage.BuildBuildingEvent -= OnBuildHouse;
                _ratingStorage.ChangeLevel -= OnChangeLevel;
                _coreGameLoad.CoreGameWinEvent -= OnCoreGameWin;
                _winCounter.NumberOfWinCountedEvent -= OnNumberOfWinCounted;
                _constructionSiteService.RefreshConstructionSitesEvent -= OnSnowdriftCleared;
                UIWindowObserver.SwitchWindowEvent -= SwitchWindowEvent;
                CutSceneStateObserver.EndCutsceneEvent -= OnEndCutScene;
                _iceCubesService.IceCubeRemovedEvent -= OnIceCubeRemovedEvent;
            }
        }

        private void OnEndCutScene(CutSceneData obj)
        {
            var condition = CreateCondition(ConditionType.EndCutScene, obj.ID);
            CreateNewConditionEvent?.Invoke(condition);
        }

        private void SwitchWindowEvent(WindowType windowType, bool isOpen)
        {
            if (!isOpen)
            {
                var condition = CreateCondition(ConditionType.CloseWindow, (int)windowType);
                CreateNewConditionEvent?.Invoke(condition);
            }
        }

        private void LoadSceneEvent()
        {
            _coroutineRunner.StartCoroutine(CreateDayCondition());
        }

        private IEnumerator CreateDayCondition()
        {
            yield return new WaitUntil(() => _timeObserver.CurrentDay != null);
            yield return new WaitForSeconds(0.5f);
            
            if (_timeObserver.CurrentDay.Month == 12)
            {
                var condition = CreateCondition(ConditionType.StartDecemberDay, _timeObserver.CurrentDay.Day);
                CreateNewConditionEvent?.Invoke(condition);
            }
            else if (_timeObserver.CurrentDay.Month == 1)
            {
                var condition = CreateCondition(ConditionType.StartJanuaryDay, _timeObserver.CurrentDay.Day);
                CreateNewConditionEvent?.Invoke(condition);
            }
            
            var openGameCondition = CreateCondition(ConditionType.OpenGame, -1);
            CreateNewConditionEvent?.Invoke(openGameCondition);
        }

        private void OnBuildHouse(BuildingType buildingType, int buildingId)
        {
            if (buildingType == BuildingType.House)
            {
                var buildHouseCondition = CreateCondition(ConditionType.BuildHouse, buildingId);
                CreateNewConditionEvent?.Invoke(buildHouseCondition);

                var buildNumberOfHouse = CreateCondition(ConditionType.BuildNumberOfHouse,
                    _buildingOnMapStorage.BuildingsOnMap.Count(b => b.Data.Type == BuildingType.House));
                CreateNewConditionEvent?.Invoke(buildNumberOfHouse);
            }
            else if (buildingType == BuildingType.Decoration)
            {
                var condition = CreateCondition(ConditionType.BuildDecoration, buildingId);
                CreateNewConditionEvent?.Invoke(condition);
            }
        }

        private void OnSnowdriftCleared(ConstructionSiteData[] all)
        {
            var condition = CreateCondition(ConditionType.ClearSnowdrift, _constructionSiteService.GetSnowdriftCount());
            CreateNewConditionEvent?.Invoke(condition);
        }

        private void OnNumberOfWinCounted(WinNumberData winNumberData)
        {
            var firstCondition = CreateCondition(ConditionType.CoreGameNumberWin, winNumberData.all_wins);
            CreateNewConditionEvent?.Invoke(firstCondition);

            var secondCondition = CreateCondition(ConditionType.CoreGameNumberWinInARow, winNumberData.wins_in_row);
            CreateNewConditionEvent?.Invoke(secondCondition);
        }


        private void OnCoreGameWin(int nextLevel)
        {
            var currentLevel = nextLevel - 1;
            var condition = CreateCondition(ConditionType.CoreGameWinLevel, currentLevel);
            CreateNewConditionEvent?.Invoke(condition);
        }

        private void OnChangeLevel(int level)
        {
            var condition = CreateCondition(ConditionType.LevelUp, level);
            CreateNewConditionEvent?.Invoke(condition);
        }

        private void OnAddCharacter(int characterId)
        {
            var condition = CreateCondition(ConditionType.CreateCharacter, characterId);
            CreateNewConditionEvent?.Invoke(condition);
        }

        private void OnIceCubeRemovedEvent(int iceCubeId)
        {
            var condition = CreateCondition(ConditionType.IceCubeRemoved, iceCubeId);
            CreateNewConditionEvent?.Invoke(condition);
        }

        private ConditionData CreateCondition(ConditionType type, int value)
        {
            var condition = new ConditionData()
            {
                Type = type,
                Value = value
            };
            return condition;
        }
    }
}