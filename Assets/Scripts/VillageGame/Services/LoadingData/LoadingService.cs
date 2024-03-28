using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGame;
using DefaultNamespace.VillageGame.Infrastructure;
using SO;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Logic.Tree;
using VillageGame.Logic.Tutorial;
using VillageGame.Services.Buildings;
using VillageGame.Services.Characters;
using VillageGame.Services.CutScenes;
using VillageGame.Services.DailyTasks;
using VillageGame.Services.Presents;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Controllers;
using VillageGame.UI.Panels;
using Web.Api;
using Web.RequestStructs;

namespace VillageGame.Services.LoadingData
{
    public class LoadingService
    {
        private readonly WebAPI _api;
        private readonly LoadingCurtainPanel _loadingCurtain;

        private readonly CurrencyStorage _currencyStorage;
        private readonly RatingStorage _ratingStorage;
        private readonly MainerService _mainingService;
        private readonly LoadBuildingService _loadBuildingService;
        private readonly CalendarPresentsStorage _calendarPresentStorage;
        private readonly CharacterSpawnService _characterSpawnService;
        private readonly CutSceneStartController _cutSceneStartController;
        private readonly WinCounter _winCounter;
        private readonly DailyTasksService _dailyTasksService;
        private readonly TimeObserver _timeObserver;
        private readonly DailyVisitService _dailyVisitService;
        private readonly PurchasedBuildingsStorage _purchasedBuildingsStorage;
        private readonly ChristmasTree _christmasTree;
        private readonly ConstructionSiteService _constructionSiteService;
        private readonly EnergyStorage _energyStorage;
        private readonly CoreGameBonusStorage _coreGameBonusStorage;
        private readonly NewYearProjectsService _newYearProjectsService;
        private readonly MainPanelNotificationController _mainPanelNotificationController;
        private readonly DialoguePanelController _dialoguePanelController;
        private readonly TutorialStateStorage _tutorialStateStorage;
        private readonly DodoCoinService _dodoCoinService;
        private readonly DodoBirdsService _dodoBirdsService;
        private readonly CoreGameLoadService _coreGameLoadService;
        private readonly IceCubesService _iceCubesService;

        public Action SceneLoad;
        private bool _isLoaded;
        private readonly CoroutineRunner _coroutineRunner;

        [Inject]
        public LoadingService(IObjectResolver objectResolver)
        {
            _api = objectResolver.Resolve<WebAPI>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();

            _cutSceneStartController = objectResolver.Resolve<CutSceneStartController>();
            _loadingCurtain = objectResolver.Resolve<UIFacade>().FindPanel<LoadingCurtainPanel>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            _mainingService = objectResolver.Resolve<MainerService>();
            _christmasTree = objectResolver.Resolve<ChristmasTree>();
            _calendarPresentStorage = objectResolver.Resolve<CalendarPresentsStorage>();
            _characterSpawnService = objectResolver.Resolve<CharacterSpawnService>();
            _loadBuildingService = new LoadBuildingService(objectResolver);
            _winCounter = objectResolver.Resolve<WinCounter>();
            _dailyTasksService = objectResolver.Resolve<DailyTasksService>();
            _dailyVisitService = objectResolver.Resolve<DailyVisitService>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();
            _purchasedBuildingsStorage = objectResolver.Resolve<PurchasedBuildingsStorage>();
            _energyStorage = objectResolver.Resolve<EnergyStorage>();
            _coreGameBonusStorage = objectResolver.Resolve<CoreGameBonusStorage>();
            _newYearProjectsService = objectResolver.Resolve<NewYearProjectsService>();
            _mainPanelNotificationController = objectResolver.Resolve<MainPanelNotificationController>();
            _dialoguePanelController = objectResolver.Resolve<DialoguePanelController>();

            _tutorialStateStorage = objectResolver.Resolve<TutorialStateStorage>();

            _dodoCoinService = objectResolver.Resolve<DodoCoinService>();
            _dodoBirdsService = objectResolver.Resolve<DodoBirdsService>();

            _coreGameLoadService = objectResolver.Resolve<CoreGameLoadService>();
            _iceCubesService = objectResolver.Resolve<IceCubesService>();


            SubscribeToEvents(true);
        }

        ~LoadingService()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _api.LoadData += LoadGame;
            }
            else
            {
                _api.LoadData -= LoadGame;
            }
        }

        private IEnumerator IELoadGame(LoadData request)
        {
            Debugging.Log($"Loading Service: -> LoadGame: is loaded = {_isLoaded}", ColorType.Aqua);
            if (_isLoaded)
            {
                _loadingCurtain.Hide();
                yield break;
            }

            yield return new WaitUntil(() => !string.IsNullOrEmpty(JSAPI.UID));

            Debugging.Log($"Loading Service: -> LoadGame: request = {request != null}", ColorType.Aqua);
            if (request == null)
            {
                request = new LoadData();
                Debugging.Log($"Loading Service: -> LoadGame: create new data", ColorType.Aqua);
            }

            StaticPrefs.SelectedCoreGameLevel = request.data.core_game_level;

            var loadTasks = GetLoadingTasksList(out var names);

            Debugging.Log($"Loading Service: -> LoadGame: await loading entity list", ColorType.Aqua);

            for (int i = 0; i < loadTasks.Count; i++)
            {
                loadTasks[i].Load(request);
                Debugging.Log($"Current loading: {names[i]}",ColorType.Aqua);
            }

            _isLoaded = true;
            Debugging.Log($"Loading Service: -> LoadGame: data is loading", ColorType.Aqua);
            SceneLoad?.Invoke();
            GlobalEvent.LoadGameInvoke();
            _loadingCurtain.Hide();
        }

        private void LoadGame(LoadData request)
            {
                _coroutineRunner.StartCoroutine(IELoadGame(request));
            }

            private List<ILoading> GetLoadingTasksList(out List<string> names)
            {
                var tasks = new List<ILoading>
                {
                    _timeObserver,
                    _constructionSiteService,
                    _loadBuildingService,
                    _ratingStorage,
                    _currencyStorage,
                    _tutorialStateStorage,
                    _christmasTree,
                    _mainingService,
                    _calendarPresentStorage,
                    _characterSpawnService,
                    _cutSceneStartController,
                    _winCounter,
                    _dailyVisitService,
                    _dailyTasksService,
                    _purchasedBuildingsStorage,
                    _energyStorage,
                    _coreGameBonusStorage,
                    _newYearProjectsService,
                    _mainPanelNotificationController,
                    _dialoguePanelController,
                    _dodoCoinService,
                    _dodoBirdsService,
                    _coreGameLoadService,
                    _iceCubesService
                };

                names = new List<string>
                {
                    " время",
                    " сугробы",
                    " домики",
                    " рейтинг",
                    " звёздочки",
                    " обучение",
                    " ёлку",
                    " шарики",
                    " подарки",
                    " жителей",
                    " катсцены",
                    " победы",
                    " задания",
                    " посещения",
                    " магазин",
                    " энергию",
                    " упаковки",
                    " проекты",
                    " уведомления",
                    " диалоги",
                    " подсказки",
                    " монетки",
                    " птиц",
                    " кор игру",
                    " ледяные блоки"
                };

                return tasks;
            }
        }
    }