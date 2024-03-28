using System;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data.Types;
using VillageGame.Services.Buildings;
using VillageGame.Services.Snowdrifts;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Controllers
{
    public class BuildModeUIPanelController : UIPanelController, IInitializable
    {
        private readonly BuildModeUIPanel _buildModeUIPanel;
        private readonly MainUIPanelController _mainUIPanelController;
        private readonly BuildObserver _buildObserver;
        private readonly ConstructionSiteService _constructionSiteService;

        public Action PressExitButton;

        [Inject]
        public BuildModeUIPanelController(IObjectResolver resolver)
        {
            _buildModeUIPanel = resolver.Resolve<UIFacade>().FindPanel<BuildModeUIPanel>();
            _uiPanel = _buildModeUIPanel;

            _constructionSiteService = resolver.Resolve<ConstructionSiteService>();

            _mainUIPanelController = resolver.Resolve<MainUIPanelController>();
            _buildObserver = resolver.Resolve<BuildObserver>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~BuildModeUIPanelController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _buildModeUIPanel.ExitModeButton.ClickEvent += OnExitModeButtonClick;
                _buildObserver.StartBuildEvent += StartBuildEvent;
                _buildObserver.StopBuildEvent += StopBuildEvent;
            }
            else
            {
                _buildModeUIPanel.ExitModeButton.ClickEvent -= OnExitModeButtonClick;
                _buildObserver.StartBuildEvent -= StartBuildEvent;
                _buildObserver.StopBuildEvent -= StopBuildEvent;
            }
        }

        private void StartBuildEvent(BuildingType buildingType)
        {
            _buildModeUIPanel.Show();
            
            if (buildingType == BuildingType.House)
            {
                _constructionSiteService.SetHighlightConstructionSites(true);
            }
            
            _mainUIPanelController.HideUIPanel();
        }

        private void StopBuildEvent()
        {
            _buildModeUIPanel.Hide();
            _constructionSiteService.SetHighlightConstructionSites(false);
            _mainUIPanelController.ShowUIPanel();
        }
        private void OnExitModeButtonClick()
        {
            _buildModeUIPanel.Hide();
            _constructionSiteService.SetHighlightConstructionSites(false);
            _mainUIPanelController.ShowUIPanel();
            PressExitButton?.Invoke();
        }
    }
}