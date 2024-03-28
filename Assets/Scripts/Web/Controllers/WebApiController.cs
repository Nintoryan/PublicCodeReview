using System;
using System.Collections.Generic;
using System.Linq;
using CoreGame;
using SO.Data;
using SO.Data.Presents;
using UnityEngine;
using VContainer;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Logic.Tree;
using VillageGame.Logic.Tutorial;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using VillageGame.Services.DailyTasks;
using VillageGame.Services.Presents;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;
using VillageGame.UI.Controllers;
using Web.Api;
using Web.ResponseStructs;
using Web.ResponseStructs.PayloadValues;

namespace Web
{
    public class WebApiController : MonoBehaviour
    {
        [SerializeField] private bool _isGameWithWebSocket;
        private WebAPI _api;

        private BuildingOnMapStorage _buildingOnMapStorage;
        private ApplicationObserver _applicationObserver;
        private RatingStorage _ratingStorage;
        private CurrencyStorage _currencyStorage;
        private CoreGameLoadService _coreGameLoadService;
        private CalendarPresentsStorage _calendarPresentsStorage;
        private CutSceneStartController _cutSceneStartController;
        private DailyTasksService _dailyTasksService;
        private TimeObserver _timeObserver;
        private WinCounter _winCounter;
        private ChristmasTree _christmasTree;
        private DailyVisitService _dailyVisitService;
        private CharactersStorage _characterStorage;
        private ConstructionSiteService _constructionSiteService;
        private PurchasedBuildingsStorage _purchasedBuildingsStorage;
        private EnergyStorage _energyStorage;
        private CoreGameBonusStorage _coreGameBonusStorage;
        private NewYearProjectsService _newYearProjectsService;
        private EnterNamePanelController _enterNamePanelController;
        private TutorialStateStorage _tutorialStateStorage;
        private DodoCoinService _dodoCoinService;
        private IceCubesService _iceCubesService;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _api = objectResolver.Resolve<WebAPI>();

            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _applicationObserver = objectResolver.Resolve<ApplicationObserver>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            _coreGameLoadService = objectResolver.Resolve<CoreGameLoadService>();
            _calendarPresentsStorage = objectResolver.Resolve<CalendarPresentsStorage>();
            _cutSceneStartController = objectResolver.Resolve<CutSceneStartController>();
            _dailyTasksService = objectResolver.Resolve<DailyTasksService>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            _winCounter = objectResolver.Resolve<WinCounter>();
            _christmasTree = objectResolver.Resolve<ChristmasTree>();
            _dailyVisitService = objectResolver.Resolve<DailyVisitService>();
            _characterStorage = objectResolver.Resolve<CharactersStorage>();
            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();
            _purchasedBuildingsStorage = objectResolver.Resolve<PurchasedBuildingsStorage>();
            _energyStorage = objectResolver.Resolve<EnergyStorage>();
            _coreGameBonusStorage = objectResolver.Resolve<CoreGameBonusStorage>();
            _newYearProjectsService = objectResolver.Resolve<NewYearProjectsService>();
            _enterNamePanelController = objectResolver.Resolve<EnterNamePanelController>();
            _tutorialStateStorage = objectResolver.Resolve<TutorialStateStorage>();
            _dodoCoinService = objectResolver.Resolve<DodoCoinService>();
            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        private void OnEnable()
        {
            if (!_isGameWithWebSocket) return;
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            if (!_isGameWithWebSocket) return;
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _calendarPresentsStorage.TakeNewClosedPresentBoxEvent += RefreshPresents;
                _calendarPresentsStorage.OpenCalendarPresentEvent += RefreshPresents;
                _constructionSiteService.RefreshConstructionSitesEvent += SnowdriftClearedEvent;
                _dailyVisitService.PressTakeDailyVisitPresentEvent += OnOpenDailyVisitsPresentEvent;
                _coreGameLoadService.CoreGameWinEvent += RefreshCoreGameData;
                _dailyTasksService.SetNewTasksListEvent += RefreshDailyTasks;
                _winCounter.NumberOfWinCountedEvent += RefreshWinNumberData;
                _cutSceneStartController.RefreshWatchedScenesEvent += EndCutSceneEvent;
                _applicationObserver.ApplicationQuitEvent += ApplicationQuit;
                _dailyTasksService.CompliteTaskEvent += RefreshDailyTasks;
                _dailyVisitService.SetVisitNumber += OnSetVisitNumberData;
                _timeObserver.SetTimeZoneEvent += RefreshPlayerTimeZone;
                _timeObserver.SetBaseUtcOffset += RefreshBaseTimeZoneOffSet;
                _buildingOnMapStorage.RemoveBuildingEvent += RefreshHousesList;
                _buildingOnMapStorage.BuildBuildingEvent += RefreshHousesList;
                _buildingOnMapStorage.MoveBuildingEvent += RefreshHousesList;
                _ratingStorage.ChangeRaiting += RefreshRating;
                _characterStorage.AddCharacter += AddCharacter;
                _currencyStorage.Change += RefreshCurrency;
                _purchasedBuildingsStorage.ChangeEvent += ChangeEvent;
                _energyStorage.EnergyUpdatedEvent += OnEnergyUpdatedEvent;
                _christmasTree.ChangeEvent += OnChristmasTreeChange;
                _coreGameBonusStorage.CoreGameShufflesUpdatedEvent += OnCoreGameShufflesUpdateEvent;
                _coreGameBonusStorage.CoreGameHintsUpdatedEvent += OnCoreGameHintsUpdateEvent;
                _newYearProjectsService.NewYearProjectsUnlockedEvent += OnNewYearProjectsUnlockedEvent;
                _newYearProjectsService.NewYearProjectsUpdatedEvent += OnNewYearProjectsUpdatedEvent;
                _enterNamePanelController.SetPlayerNameEvent += OnSetPlayerNameEvent;
                _tutorialStateStorage.RefreshTutorialsDataEvent += OnRefreshTutorialsDataEvent;
                
                DodoCoinService.SendCoins += OnSendCoins;
                DodoCoinService.RemoveCoins += OnRemoveCoins;
                
                PromoCodeService.IssuePromoCode += OnIssuePromoCode;
                
                DodoBirdsService.BalanceUpdated += OnBirdsBalance;
                DodoBirdsService.GiftsUpdated += OnGiftsUpdated;
                
                RealLifeService.TrackOrderPrice += OnSetTrackingOrderPrice;
                RealLifeService.TrackProductGuid += OnSetTrackingProductId;

                _coreGameLoadService.LastStartedCoreGameLevelUpdatedEvent += OnLastStartedCoreGameLevelUpdatedEvent;
                LevelStorage.CoreGameWonEvent += OnCoreGameFinishedEvent;
                LevelTimer.CoreGameDefeatedEvent += OnCoreGameFinishedEvent;

                WherePresentsService.WherePresentsWatched += OnWherePresentsWatched;
                WherePresentsService.YearPizzaInfoWatched += OnYearPizzaInfoWatched;

                _iceCubesService.IceCubesDataUpdatedEvent += OnIceCubesDataUpdatedEvent;
            }
            else
            {
                _calendarPresentsStorage.TakeNewClosedPresentBoxEvent -= RefreshPresents;
                _calendarPresentsStorage.OpenCalendarPresentEvent -= RefreshPresents;
                _constructionSiteService.RefreshConstructionSitesEvent -= SnowdriftClearedEvent;
                _dailyVisitService.PressTakeDailyVisitPresentEvent -= OnOpenDailyVisitsPresentEvent;
                _coreGameLoadService.CoreGameWinEvent -= RefreshCoreGameData;
                _dailyTasksService.SetNewTasksListEvent -= RefreshDailyTasks;
                _applicationObserver.ApplicationQuitEvent -= ApplicationQuit;
                _winCounter.NumberOfWinCountedEvent -= RefreshWinNumberData;
                _cutSceneStartController.RefreshWatchedScenesEvent -= EndCutSceneEvent;
                _dailyVisitService.SetVisitNumber -= OnSetVisitNumberData;
                _timeObserver.SetTimeZoneEvent -= RefreshPlayerTimeZone;
                _timeObserver.SetBaseUtcOffset -= RefreshBaseTimeZoneOffSet;
                _dailyTasksService.CompliteTaskEvent -= RefreshDailyTasks;
                _buildingOnMapStorage.BuildBuildingEvent -= RefreshHousesList;
                _buildingOnMapStorage.RemoveBuildingEvent -= RefreshHousesList;
                _buildingOnMapStorage.MoveBuildingEvent -= RefreshHousesList;
                _ratingStorage.ChangeRaiting -= RefreshRating;
                _characterStorage.AddCharacter -= AddCharacter;
                _currencyStorage.Change -= RefreshCurrency;
                _purchasedBuildingsStorage.ChangeEvent -= ChangeEvent;
                _energyStorage.EnergyUpdatedEvent -= OnEnergyUpdatedEvent;
                _christmasTree.ChangeEvent -= OnChristmasTreeChange;
                _coreGameBonusStorage.CoreGameShufflesUpdatedEvent -= OnCoreGameShufflesUpdateEvent;
                _coreGameBonusStorage.CoreGameHintsUpdatedEvent -= OnCoreGameHintsUpdateEvent;
                _newYearProjectsService.NewYearProjectsUnlockedEvent -= OnNewYearProjectsUnlockedEvent;
                _newYearProjectsService.NewYearProjectsUpdatedEvent -= OnNewYearProjectsUpdatedEvent;
                _enterNamePanelController.SetPlayerNameEvent -= OnSetPlayerNameEvent;
                _tutorialStateStorage.RefreshTutorialsDataEvent -= OnRefreshTutorialsDataEvent;
                
                DodoCoinService.SendCoins -= OnSendCoins;
                DodoCoinService.RemoveCoins -= OnRemoveCoins;

                PromoCodeService.IssuePromoCode -= OnIssuePromoCode;
                
                DodoBirdsService.BalanceUpdated -= OnBirdsBalance;
                DodoBirdsService.GiftsUpdated -= OnGiftsUpdated;
                
                RealLifeService.TrackOrderPrice -= OnSetTrackingOrderPrice;
                RealLifeService.TrackProductGuid -= OnSetTrackingProductId;

                _coreGameLoadService.LastStartedCoreGameLevelUpdatedEvent -= OnLastStartedCoreGameLevelUpdatedEvent;
                LevelStorage.CoreGameWonEvent -= OnCoreGameFinishedEvent;
                LevelTimer.CoreGameDefeatedEvent -= OnCoreGameFinishedEvent;
                
                WherePresentsService.WherePresentsWatched -= OnWherePresentsWatched;
                WherePresentsService.YearPizzaInfoWatched -= OnYearPizzaInfoWatched;

                _iceCubesService.IceCubesDataUpdatedEvent -= OnIceCubesDataUpdatedEvent;
            }
        }

        private void OnGiftsUpdated()
        {
            _api.SendBirdGiftsUpdated(DodoBirdsService.Gifts);
        }

        private void OnBirdsBalance()
        {
            _api.SendBirdsBalanceUpdated(DodoBirdsService.Balance);
        }

        private void OnSendCoins(AddCoinsType coinsType,string idempotencyKey)
        {
            _api.SendAddDodoCoins(coinsType,idempotencyKey);
        }
        
        private void OnRemoveCoins(int amount, string idempotencyKey)
        {
            _api.SendRemoveDodoCoins(amount,idempotencyKey);
        }

        private void OnIssuePromoCode(int giftID, string idempotencyKey)
        {
            _api.SendIssuePromocode(giftID,idempotencyKey);
        }

        private void OnSetTrackingProductId(int taskID, string productGuid)
        {
            _api.SendTrackProductID(taskID,productGuid);
        }

        private void OnSetTrackingOrderPrice(int taskId, int orderPrice)
        {
            _api.SendTrackOrderPrice(taskId,orderPrice);
        }


        private void OnRefreshTutorialsDataEvent()
        {
            _api.SendVillageTutorial(_tutorialStateStorage.TutorialData);
        }

        private void OnSetPlayerNameEvent(string playerName)
        {
            _api.SendPlayerName(playerName);
        }

        private void OnChristmasTreeChange()
        {
            _api.SendChristmasTreeBank(_christmasTree.Bank);
        }

        private void ChangeEvent()
        {
            _api.SendPurchasedBuildings(PurchasedBuildingsStorage.PurchasedBuildings);
        }

        private void SnowdriftClearedEvent(ConstructionSiteData[] constructionSites)
        {
            _api.SendConstructionSite(constructionSites);
        }

        private void AddCharacter(int _)
        {
            var existingCharactersTypes = _characterStorage.ExistingCharacters.Select(c => c.Data.Type);
            _api.SendExistingCharacters(existingCharactersTypes.ToList());
        }

        private void OnSetVisitNumberData()
        {
            _api.SendDailyVisitsPresentData(_dailyVisitService.VisitData);
        }

        private void OnOpenDailyVisitsPresentEvent(PresentValueData _)
        {
            _api.SendDailyVisitsPresentData(_dailyVisitService.VisitData);
        }
        
        private void RefreshWinNumberData(WinNumberData winNumberData)
        {
            _api.SendWinNumberData(winNumberData);
        }

        private void RefreshPlayerTimeZone(TimeZoneInfo timeZone)
        {
            _api.SendPlayerTimeZone(timeZone);
        }

        private void RefreshBaseTimeZoneOffSet(int baseUtcOffset)
        {
            _api.SendBaseUtcOffset(baseUtcOffset);
        }

        private void RefreshDailyTasks(DailyTaskData _)
        {
            _api.SendDailyTasks(_dailyTasksService.TodayTasksState);
        }

        private void RefreshDailyTasks(TodayTasksState obj)
        {
            _api.SendDailyTasks(_dailyTasksService.TodayTasksState);
        }

        private void RefreshPresents(List<OpenedPresentBoxData> presents, PresentBoxData _)
        {
            _api.SendPresentData(_calendarPresentsStorage.OpenedPresentsData);
        }

        private void RefreshPresents(PresentBoxData _)
        {
            _api.SendPresentData(_calendarPresentsStorage.OpenedPresentsData);
        }

        private void RefreshCoreGameData(int level)
        {
            _api.SendCoreGameLevel(level);
        }

        private void RefreshCurrency(int value)
        {
            _api.SendCurrencyData(value);
        }

        private void OnEnergyUpdatedEvent(EnergyData energyData)
        {
            _api.SendEnergy(energyData);
        }

        private void OnCoreGameShufflesUpdateEvent(int value)
        {
            _api.SendCoreGameShuffles(value);
        }

        private void OnCoreGameHintsUpdateEvent(int value)
        {
            _api.SendCoreGameHints(value);
        }

        private void RefreshRating(int _, int __)
        {
            var levelData = new PlayerProgressData()
            {
                level = _ratingStorage.Level,
                rating = _ratingStorage.Raiting
            };

            _api.SendPlayerProgressData(levelData);
        }

        private void EndCutSceneEvent(List<WatchedScenesData> watchedData)
        {
            _api.SendCutScenesData(watchedData);
        }

        private void ApplicationQuit()
        {
            var date = _timeObserver.CurrentPlayerTime.ToString();
            _api.SendExitTimeData(date);
        }

        private void RefreshHousesList(BuildingType buildingType, int _)
        {
            _api.SendBuildingsData(_buildingOnMapStorage.BuildingsOnMapData);
            _api.SendConstructionSite(_constructionSiteService.GetAllSitesData());
        }

        private void OnNewYearProjectsUnlockedEvent()
        {
            _api.SendNewYearProjectsUnlocked(true);
        }

        private void OnNewYearProjectsUpdatedEvent(List<int> projectsStates)
        {
            _api.SendNewYearProjectsData(projectsStates);
        }

        private void OnLastStartedCoreGameLevelUpdatedEvent(int lastPlayedCoreGameLevel)
        {
            _api.SendLastStartedCoreGameLevelUpdated(lastPlayedCoreGameLevel);
        }

        private void OnCoreGameFinishedEvent(bool isVictory, int coreGameRealLevel)
        {
            _api.SendCoreGameLevelAnalytics(isVictory,coreGameRealLevel);
        }

        private void OnWherePresentsWatched()
        {
            _api.SendWherePresentsWatched();
        }

        private void OnYearPizzaInfoWatched()
        {
            _api.SendYearPizzaInfoWatched();
        }

        private void OnIceCubesDataUpdatedEvent(IceCubesData iceCubesData)
        {
            _api.SendIceCubesDataUpdated(iceCubesData);
        }
    }
}