using System;
using System.Collections.Generic;
using System.Linq;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.UI.Panels;
using IInitializable = VContainer.Unity.IInitializable;

namespace VillageGame.UI.Services
{
    public class UIWindowObserver : IInitializable
    {
        private readonly List<UIPanel> _windows;
        
        public static event Action<WindowType, bool> SwitchWindowEvent;
        
        public static UIPanel CurrentOpenedPanel { get; private set; }
        

        [Inject]
        public UIWindowObserver(IObjectResolver objectResolver)
        {
            var uiFacade = objectResolver.Resolve<UIFacade>();
            _windows = uiFacade.Panels.Where(w => w.IsWindow).ToList();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~UIWindowObserver()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                foreach (var uiPanel in _windows)
                {
                    uiPanel.PanelSwitchEvent += OnPanelSwitch;
                }
            }
            else
            {
                foreach (var uiPanel in _windows)
                {
                    uiPanel.PanelSwitchEvent -= OnPanelSwitch;
                }
            }
        }
        
        private void OnPanelSwitch(UIPanel panel, bool isOpen)
        {
            var oldPanel = CurrentOpenedPanel;
            
            if (CurrentOpenedPanel == panel && isOpen)
            {
                return;
            }
            
            if (CurrentOpenedPanel == panel && !isOpen)
            {
                CurrentOpenedPanel = null;
            }

            if (CurrentOpenedPanel != panel && isOpen)
            {
                CurrentOpenedPanel = panel;
                
                if (oldPanel != null)
                {
                    oldPanel.Hide(); 
                }
            }

            if (oldPanel != CurrentOpenedPanel)
            {
                SwitchWindowEvent?.Invoke(panel.Type, isOpen);
            }
        }
    }
}