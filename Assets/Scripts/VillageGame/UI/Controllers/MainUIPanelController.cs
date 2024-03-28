using VContainer;
using VillageGame.Services;
using VillageGame.Services.Storages;
using VillageGame.UI.Panels;
using VillageGame.UI.Panels.Warnings;
using Web.ResponseStructs;

namespace VillageGame.UI.Controllers
{
    public class MainUIPanelController : UIPanelController
    {
        private readonly MainUIPanel _mainUIPanel;

        private readonly CurrencyStorage _currencyStorage;
        private readonly RatingStorage _ratingStorage;
        private readonly ApplicationObserver _applicationObserver;
        private readonly CoreGameLoadService _coreGameLoadService;
        private readonly EnergyStorage _energyStorage;
        private readonly NewYearProjectsService _newYearProjectsService;
        private readonly NoStarsPanel _noStarsPanel;
        private readonly EnergyWarningUIPanel _energyWarningUIPanel;
        

        [Inject]
        public MainUIPanelController(IObjectResolver resolver)
        {
            _mainUIPanel = resolver.Resolve<UIFacade>().FindPanel<MainUIPanel>();
            _noStarsPanel = resolver.Resolve<UIFacade>().FindPanel<NoStarsPanel>();
            _energyWarningUIPanel = resolver.Resolve<UIFacade>().FindPanel<EnergyWarningUIPanel>();
            _uiPanel = _mainUIPanel;
            _currencyStorage = resolver.Resolve<CurrencyStorage>();
            _ratingStorage = resolver.Resolve<RatingStorage>();
            _applicationObserver = resolver.Resolve<ApplicationObserver>();
            _coreGameLoadService = resolver.Resolve<CoreGameLoadService>();
            _energyStorage = resolver.Resolve<EnergyStorage>();
            _newYearProjectsService = resolver.Resolve<NewYearProjectsService>();
            
            SubscribeToEvents(true);
            _mainUIPanel.SetNewYearProjectsButton(false);
        }

        ~MainUIPanelController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _applicationObserver.SceneLoadEvent += OnSceneLoad;
                _currencyStorage.Change += OnUpdateCurrencyValue;
                _ratingStorage.ChangeLevel += OnUpdateLevelValue;
                _ratingStorage.ChangeRaiting += OnChangeRating;
                _mainUIPanel.CoreGameButton.ClickEvent += OnPressCoreGameButton;
                _noStarsPanel.PlayButton.ClickEvent += OnPressCoreGameButton;
                _energyWarningUIPanel.EnergyRestoredToPlay += OnPressCoreGameButton;
                _energyStorage.EnergySettedEvent += OnEnergySettedEvent;
                _newYearProjectsService.NewYearProjectsActivatedEvent += OnNewYearProjectsActivatedEvent;
            }
            else
            {
                _applicationObserver.SceneLoadEvent += OnSceneLoad;
                _currencyStorage.Change -= OnUpdateCurrencyValue;
                _ratingStorage.ChangeLevel -= OnUpdateLevelValue;
                _ratingStorage.ChangeRaiting -= OnChangeRating;
                _mainUIPanel.CoreGameButton.ClickEvent -= OnPressCoreGameButton;
                _noStarsPanel.PlayButton.ClickEvent -= OnPressCoreGameButton;
                _energyWarningUIPanel.EnergyRestoredToPlay -= OnPressCoreGameButton;
                _energyStorage.EnergySettedEvent -= OnEnergySettedEvent;
                _newYearProjectsService.NewYearProjectsActivatedEvent -= OnNewYearProjectsActivatedEvent;
            }
        }

        private void OnNewYearProjectsActivatedEvent()
        {
            _mainUIPanel.SetNewYearProjectsButton(true);
        }

        private void OnSceneLoad()
        {
            _mainUIPanel.UpdateCurrencyValue(_currencyStorage.Current);
            _mainUIPanel.UpdateLevelValue(_ratingStorage.Level);
            _mainUIPanel.UpdateRatingValue(_ratingStorage.Raiting, _ratingStorage.RatingRequirement);
        }

        private void OnChangeRating(int current, int max)
        {
            _mainUIPanel.UpdateRatingValue(current, max);
        }


        private void OnPressCoreGameButton()
        {
            _noStarsPanel.HideNoAnimation();
            _coreGameLoadService.LoadCoreGameScene();
        }

        private void OnEnergySettedEvent(int value)
        {
            _mainUIPanel.UpdateEnergyValue(value);
        }


        #region Tests

        private void OnUpdateCurrencyValue(int value)
        {
            _mainUIPanel.UpdateCurrencyValue(value);
        }

        public void OnUpdateLevelValue(int value)
        {
            _mainUIPanel.UpdateLevelValue(value);
        }

        #endregion
    }
}