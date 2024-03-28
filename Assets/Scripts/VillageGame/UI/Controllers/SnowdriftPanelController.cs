using System;
using Data.Scripts.Audio;
using SO;
using VContainer;
using VillageGame.Logic.Snowdrifts;
using VillageGame.Services.Storages;
using VillageGame.UI.Panels;
using VillageGame.UI.Panels.Warnings;
using IInitializable = VContainer.Unity.IInitializable;

namespace VillageGame.UI.Controllers
{
    public class SnowdriftPanelController : IInitializable
    {
        private readonly SnowdriftPanel _snowdriftPanel;
        private readonly ConstructionSiteWarningPanel _warningPanel;
        
        private readonly BuildingsConfig _buildingConfig;
        private readonly CurrencyStorage _currencyStorage;

        private readonly NoStarsPanel _noStarsPanel;
        private ConstructionSite _contextConstructionSite;
        private int _contextPrice;

        public Action<ConstructionSite> SnowdriftClearedEvent;

        [Inject]
        public SnowdriftPanelController(IObjectResolver objectResolver)
        {
            _snowdriftPanel = objectResolver.Resolve<UIFacade>().FindPanel<SnowdriftPanel>();
            _warningPanel = objectResolver.Resolve<UIFacade>().FindWarningPanel<ConstructionSiteWarningPanel>();
            _noStarsPanel = objectResolver.Resolve<UIFacade>().FindPanel<NoStarsPanel>();
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            _buildingConfig = objectResolver.Resolve<BuildingsConfig>();
        }

        ~SnowdriftPanelController()
        {
            SubscribeToEvents(false);
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _snowdriftPanel.BuyButton.ClickEvent += OnClickBuyButton;
                _snowdriftPanel.CloseButton.ClickEvent += OnClickCloseButton;
            }
            else
            {
                _snowdriftPanel.BuyButton.ClickEvent -= OnClickBuyButton;
                _snowdriftPanel.CloseButton.ClickEvent -= OnClickCloseButton;
            }
        }

        public void ShowPanel(ConstructionSite constructionSite)
        {
            _contextConstructionSite = constructionSite;
            _contextPrice = _contextConstructionSite.Data.PositionType == ConstructionSite.PositionType.Distant
                ? _buildingConfig.DistantSnowdriftPrice
                : _buildingConfig.NearbySnowdriftPrice;

            _snowdriftPanel.SetPrice(_contextPrice.ToString());
            _snowdriftPanel.Show();
        }


        public void ShowBuildWarningPanel()
        {
             _warningPanel.Show();   
        }
        
        
        private void OnClickBuyButton()
        {
            if (_currencyStorage.IsEnoughCurrency(_contextPrice))
            {
                _currencyStorage.Remove(_contextPrice);
                _snowdriftPanel.Hide();
                SnowdriftClearedEvent?.Invoke(_contextConstructionSite);
               AudioManager.Instance.PlayAudioEvent(AudioEventType.ClearSnowdrift);
                Reset();
            }
            else
            {
                _noStarsPanel.Show();
            }
        }

        private void OnClickCloseButton()
        {
            _snowdriftPanel.Hide();
            Reset();
        }

        private void Reset()
        {
            _contextPrice = 0;
            _contextConstructionSite = null;
        }
    }
}