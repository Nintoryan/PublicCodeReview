using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.VillageGame.Infrastructure;
using SO;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Snowdrifts;
using VillageGame.UI.Panels;
using VillageGame.UI.Services;
using Web.RequestStructs;

namespace VillageGame.Services.CutScenes
{
    public class CutSceneStartController : ILoading
    {
        private readonly CutSceneConfig _cutSceneConfig;
        private readonly ConditionProvider _conditionsProvider;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly BigSnowdrifts _bigSnowDrift;

        private List<WatchedScenesData> WatchedSceneIndexes = new();

        public Action<List<WatchedScenesData>> RefreshWatchedScenesEvent;

        private bool _isLoaded;


        [Inject]
        public CutSceneStartController(IObjectResolver objectResolver)
        {
            _cutSceneConfig = objectResolver.Resolve<CutSceneConfig>();
            _conditionsProvider = objectResolver.Resolve<ConditionProvider>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();
            _bigSnowDrift = objectResolver.Resolve<BigSnowdrifts>();

            SubscribeToEvents(true);
        }

        ~CutSceneStartController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _conditionsProvider.CreateNewConditionEvent += OnFulfilledConditionEvent;
                GlobalEvent.LoadGameEvent += LoadSceneEvent;
            }
            else
            {
                _conditionsProvider.CreateNewConditionEvent -= OnFulfilledConditionEvent;
                GlobalEvent.LoadGameEvent -= LoadSceneEvent;
            }
        }

        private void LoadSceneEvent()
        {
            _coroutineRunner.StartCoroutine(TryStartLastCutScene());
        }

        private IEnumerator TryStartLastCutScene()
        {
            if (!_cutSceneConfig.IsShowCutscene)
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();
                var nextCutSceneIndex = GetLastWatchedSceneId() + 1;
            if (!TutorialSceneIsWatched())
            {
                Debugging.Log($"CutsceneStart controller: TryStartTutorialCutScene ->  next {nextCutSceneIndex}",
                    ColorType.Magenta);
                CutSceneStateObserver.InvokeStartEvent(_cutSceneConfig.GetCutSceneData(nextCutSceneIndex));
            }
            else if(nextCutSceneIndex is > 21 and < 24)
            {
                 CutSceneStateObserver.InvokeStartEvent(_cutSceneConfig.GetCutSceneData(21));
            }

            _isLoaded = true;
        }

        private void OnFulfilledConditionEvent(ConditionData condition)
        {
            if (!_cutSceneConfig.IsShowCutscene)
            {
                return;
            }

            _coroutineRunner.StartCoroutine(StartCheckCondition(condition));
        }

        private IEnumerator StartCheckCondition(ConditionData condition)
        {
            if (CutSceneStateObserver.IsWatching)
            {
                Debugging.Log($"CutsceneStart controller: StartCheckCondition ->  Is Watching right now",
                    ColorType.Magenta);
                yield break;
            }

            yield return new WaitUntil(IsReadyShowCutscene);

            Debugging.Log($"CutsceneStart controller: StartCheckCondition ->  last watched {GetLastWatchedSceneId()} " +
                          $"condition = {condition.Type} {condition.Value}", ColorType.Magenta);

            if (_cutSceneConfig.TryGetCutSceneData(condition, GetLastWatchedSceneId(), out var cutScene))
            {
                var watchedData = WatchedSceneIndexes.FirstOrDefault(c => cutScene.ID == c.ID);

                yield return new WaitUntil(IsReadyShowCutscene);
                Debugging.Log($"CutsceneStart controller: StartCheckCondition -> " +
                              $"watched scene null = {watchedData == null} id  {watchedData?.ID} is watched {watchedData?.IsWatched}",
                    ColorType.Magenta);

                if (watchedData == null || !watchedData.IsWatched)
                {
                    var watchedLowerIDs = WatchedSceneIndexes.Where(c => c.ID < cutScene.ID).ToList();
                    var possibleLowerIDs = Enumerable.Range(0, cutScene.ID).ToList();

                    if (cutScene.ID > 0 && (!watchedLowerIDs.Any() ||
                                            !watchedLowerIDs.All(w => possibleLowerIDs.Contains(w.ID))))
                    {
                        Debugging.Log($"CutSceneStartController: StartCheckCondition -> " +
                                      "We are not launching the cat scene " +
                                      "because there are no viewed scenes with lower IDs. Last ID = {GetLastWatchedSceneId()}",
                            ColorType.Magenta);
                    }
                    else
                    {
                        Debugging.Log($"CutsceneStart controller: StartCheckCondition -> " +
                                      $"Start id {cutScene.ID}", ColorType.Magenta);

                        if (CutSceneStateObserver.IsWatching)
                        {
                            yield break;
                        }

                        CutSceneStateObserver.InvokeStartEvent(cutScene);
                    }
                }
            }
        }

        public bool CutSceneIsWatched(int cutsceneID)
        {
            var cutscene = WatchedSceneIndexes?.FirstOrDefault(s => s.ID == cutsceneID);
            return !_cutSceneConfig.IsShowCutscene || cutscene is { IsWatched: true };
        }

        public bool CutSceneIsWatched(IEnumerable<WatchedScenesData> watchedScenesDatas, int cutsceneID)
        {
            var cutscene = watchedScenesDatas?.FirstOrDefault(s => s.ID == cutsceneID);
            return !_cutSceneConfig.IsShowCutscene || cutscene is { IsWatched: true };
        }


        public void SetWatched(int id)
        {
            WatchedSceneIndexes.Add(new WatchedScenesData()
            {
                ID = id,
                IsWatched = true
            });

            var nextWatchedScenes = WatchedSceneIndexes.Where(s => s.ID > id && s.IsWatched);
            foreach (var nextWatchedScene in nextWatchedScenes)
            {
                nextWatchedScene.IsWatched = false;
            }
            RefreshWatchedScenesEvent?.Invoke(WatchedSceneIndexes);
        }

        public bool TutorialSceneIsWatched()
        {
            return GetLastWatchedSceneId() >= 4;
        }

        public int GetLastWatchedSceneId()
        {
            var maxWatchedScene = WatchedSceneIndexes
                .Where(scene => scene.IsWatched)
                .OrderByDescending(scene => scene.ID)
                .FirstOrDefault();

            return maxWatchedScene?.ID ?? -1;
        }

        public bool TutorialSceneIsWatched(List<WatchedScenesData> watchedScenesDatas)
        {
            return GetLastWatchedSceneId(watchedScenesDatas) >= 4;
        }

        private int GetLastWatchedSceneId(List<WatchedScenesData> watchedScenesDatas)
        {
            var maxWatchedScene = watchedScenesDatas
                .Where(scene => scene.IsWatched)
                .OrderByDescending(scene => scene.ID)
                .FirstOrDefault();

            return maxWatchedScene?.ID ?? -1;
        }


        public void Load(LoadData request)
        {
            WatchedSceneIndexes = request.data.scenes_watched_indexes ?? new List<WatchedScenesData>();

            _bigSnowDrift.SetGiantSnowMan(CutSceneIsWatched(23));
        }

        private bool IsReadyShowCutscene()
        {
            return _isLoaded && UIWindowObserver.CurrentOpenedPanel is null or DialogueUIPanel;
        }
    }


    [Serializable]
    public class WatchedScenesData
    {
        public int ID;
        public bool IsWatched;
    }
}