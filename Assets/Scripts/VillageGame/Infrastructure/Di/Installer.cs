using CoreGame;
using CoreGame.SO;
using SO;
using Tutorial;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data.Types;
using VillageGame.Infrastructure.Factories;
using VillageGame.Logic.Cameras;
using VillageGame.Logic.Tree;
using VillageGame.Logic.Tutorial;
using VillageGame.Services;
using VillageGame.Services.Buildings;
using VillageGame.Services.Characters;
using VillageGame.Services.CutScenes;
using VillageGame.Services.CutScenes.CustomActions;
using VillageGame.Services.DailyTasks;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Presents;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Controllers;
using VillageGame.UI.Panels;
using VillageGame.UI.Services;
using Web.Api;

namespace VillageGame.Infrastructure.Di
{
    public class Installer : LifetimeScope
    {
        [Space, Header("Scriptable objects")] 
        [SerializeField] private BuildingsConfig _buildingsConfig;
        [SerializeField] private TilesConfig _tilesConfig;
        [SerializeField] private ProgressionConfig _progressionConfig;
        [SerializeField] private PlayerPrefsConfig _playerPrefsConfig;
        [SerializeField] private DailyVisitPresentsConfig _dailyVisitPresentsConfig;
        [SerializeField] private PresentBoxesConfig _presentBoxesConfig;
        [SerializeField] private CharactersConfig _charactersConfig;
        [SerializeField] private CutSceneConfig _cutSceneConfig;
        [SerializeField] private DailyTasksConfig _dailyTasksConfig;
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private EnergyConfig _energyConfig;
        [SerializeField] private VFXConfig _vfxConfig;
        [SerializeField] private NewYearProjectsDatabase _newYearProjectsDatabase;
        [SerializeField] private LevelDatabase _levelDatabase;
        [SerializeField] private DodoBirdsGiftsConfig _dodoBirdsGiftsConfig;
        

        protected override void Configure(IContainerBuilder builder)
        {
            BindScriptableObjects(builder);
            BindUIFacade(builder);
            BindFactories(builder);
            BindStorages(builder);
            BindUIControllers(builder);
            BindCharactersServices(builder);
            BindBuildingsServices(builder);
            BindGameServices(builder);
            BindWebApi(builder);
            BindTutorialStuff(builder);
            BindCutsceneServices(builder);

            builder.RegisterComponentInHierarchy<ChristmasTree>().AsSelf(); 
            builder.RegisterComponentInHierarchy<CutSceneCamera>().AsSelf();
            builder.RegisterComponentInHierarchy<CutSceneAnimator>().AsSelf();
        }

        private void BindWebApi(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<WebAPI>().AsSelf();
            builder.RegisterComponentInHierarchy<JSAPI>().AsSelf();
        }

        private void BindScriptableObjects(IContainerBuilder builder)
        {
            builder.RegisterInstance(_buildingsConfig);
            builder.RegisterInstance(_tilesConfig);
            builder.RegisterInstance(_progressionConfig);
            builder.RegisterInstance(_playerPrefsConfig);
            builder.RegisterInstance(_dailyVisitPresentsConfig);
            builder.RegisterInstance(_presentBoxesConfig);
            builder.RegisterInstance(_charactersConfig);
            builder.RegisterInstance(_cutSceneConfig);
            builder.RegisterInstance(_dailyTasksConfig);
            builder.RegisterInstance(_audioConfig);
            builder.RegisterInstance(_energyConfig);
            builder.RegisterInstance(_newYearProjectsDatabase);
            builder.RegisterInstance(_levelDatabase);
            builder.RegisterInstance(_dodoBirdsGiftsConfig);
            builder.RegisterInstance(_vfxConfig);
        }

        private void BindStorages(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BuildingOnMapStorage>().AsSelf();
            builder.RegisterEntryPoint<PurchasedBuildingsStorage>().AsSelf();
            builder.RegisterEntryPoint<RatingStorage>().AsSelf();
            builder.RegisterEntryPoint<CurrencyStorage>().AsSelf();
            builder.RegisterEntryPoint<LoadingEntitiesStorage>().AsSelf();
            builder.RegisterEntryPoint<CharactersStorage>().AsSelf();
            builder.RegisterEntryPoint<EnergyStorage>().AsSelf();
            builder.RegisterEntryPoint<CoreGameBonusStorage>().AsSelf();
        }

        private void BindFactories(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BuildingFactory>().AsSelf();
            builder.RegisterEntryPoint<UIFactory>().AsSelf();
            builder.RegisterEntryPoint<CharactersFactory>().AsSelf();
        }

        private void BindUIFacade(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<UIFacade>().AsSelf();
            builder.RegisterEntryPoint<UIWindowObserver>().AsSelf();
        }

        private void BindUIControllers(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BuildShopUIPanelController>().AsSelf();
            builder.RegisterEntryPoint<MainUIPanelController>().AsSelf();
            builder.RegisterEntryPoint<BuildModeUIPanelController>().AsSelf();
            builder.RegisterEntryPoint<CalendarPanelController>().AsSelf();
            builder.RegisterEntryPoint<DialoguePanelController>().AsSelf();
            builder.RegisterEntryPoint<DailyTasksPanelController>().AsSelf();
            builder.RegisterEntryPoint<LevelUpPanelController>().AsSelf();
            builder.RegisterEntryPoint<DailyVisitPanelController>().AsSelf();
            builder.RegisterEntryPoint<PresentInformationPanelController>().AsSelf();
            builder.RegisterEntryPoint<SnowdriftPanelController>().AsSelf();
            builder.RegisterEntryPoint<NoDecorationSpacePanelController>().AsSelf();
            builder.RegisterEntryPoint<EnergyWarningPanelController>().AsSelf();
            builder.RegisterEntryPoint<NewYearProjectsService>().AsSelf();
            builder.RegisterEntryPoint<MainPanelNotificationController>().AsSelf();
            builder.RegisterEntryPoint<EnterNamePanelController>().AsSelf();
            builder.RegisterEntryPoint<MainButtonsController>().AsSelf();
        }

        private void BindGameServices(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ApplicationObserver>().AsSelf();
            builder.RegisterComponentInHierarchy<CoroutineRunner>().AsSelf();
            builder.RegisterEntryPoint<LoadingService>().AsSelf();
            builder.RegisterEntryPoint<InputService>().AsSelf();
            builder.RegisterComponentInHierarchy<InputBlockService>().AsSelf();
            builder.RegisterComponentInHierarchy<CoreGameLoadService>().AsSelf();
            builder.RegisterEntryPoint<SceneLoaderService>().AsSelf();
            
            builder.RegisterEntryPoint<TimeObserver>().AsSelf();
            builder.RegisterEntryPoint<ProgressionService>().AsSelf();
            builder.RegisterEntryPoint<ConditionProvider>().AsSelf();
            builder.RegisterEntryPoint<DodoAnalyticsService>().AsSelf();

            builder.RegisterEntryPoint<WinCounter>().AsSelf();
            builder.RegisterEntryPoint<RewardService>().AsSelf();
            builder.RegisterEntryPoint<DailyVisitService>().AsSelf();
            builder.RegisterEntryPoint<DailyTasksService>().AsSelf();
            builder.Register<CalendarPresentsStorage>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<EnergyRestorerService>().AsSelf();
            builder.RegisterEntryPoint<DodoCoinService>().AsSelf();
            builder.RegisterEntryPoint<DodoBirdsService>().AsSelf();
            builder.RegisterEntryPoint<VFXService>().AsSelf();
            builder.RegisterEntryPoint<VFXController>().AsSelf();

            builder.RegisterComponentInHierarchy<IceCubesService>().AsSelf();
        }

        private void BindCutsceneServices(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CutSceneStartController>().AsSelf();
            builder.RegisterEntryPoint<CutSceneActionsExecutor>().AsSelf();
            builder.RegisterEntryPoint<CutSceneController>().AsSelf();
        }
        private void BindCharactersServices(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CharactersNavigationService>().AsSelf();
            builder.RegisterEntryPoint<CharacterSpawnService>().AsSelf();
        }
        
        private void BindBuildingsServices(IContainerBuilder builder)
        {
            builder.Register<BuildObserver>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<BuildService>().AsSelf();
            builder.RegisterComponentInHierarchy<TileAreasService>().AsSelf();
            builder.RegisterEntryPoint<MainerService>().AsSelf();
            builder.RegisterComponentInHierarchy<BuildingAreaService>().AsSelf();
            builder.RegisterComponentInHierarchy<ConstructionSiteService>().AsSelf();
            builder.RegisterComponentInHierarchy<BigSnowdrifts>().AsSelf();
        }

        private void BindTutorialStuff(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TutorialStateStorage>().AsSelf();
            builder.RegisterComponentInHierarchy<TutorialFinger>().AsSelf();
        }
    }
}