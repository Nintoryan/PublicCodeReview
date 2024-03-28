using System;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Controllers
{
    public class EnterNamePanelController : IInitializable
    {
        private readonly EnterNamePanel _enterNamePanel;
        public Action<string> SetPlayerNameEvent;
        
        [Inject]
        public EnterNamePanelController(IObjectResolver objectResolver)
        {
            _enterNamePanel = objectResolver.Resolve<UIFacade>().FindPanel<EnterNamePanel>();
        }
        
        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~EnterNamePanelController()
        {
            SubscribeToEvents(false);
        }
        
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _enterNamePanel.OkButton.ClickEvent += ClickEvent;        
            }
            else
            {
                _enterNamePanel.OkButton.ClickEvent -= ClickEvent;
            }
        }

        public void Show()
        {
            _enterNamePanel.Show();
        }

        private void ClickEvent()
        {
            if (_enterNamePanel.IsNameValid())
            {
                _enterNamePanel.Hide();
                _enterNamePanel.OkButton.PlayImpact();
                SetPlayerNameEvent?.Invoke(_enterNamePanel.Input);
            }
            else
            {
                _enterNamePanel.OkButton.PlayFailureImpact();
            }
        }
    }
}