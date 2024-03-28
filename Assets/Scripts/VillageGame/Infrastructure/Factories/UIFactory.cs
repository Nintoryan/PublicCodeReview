using UnityEngine;
using VContainer;
using VillageGame.UI;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;

namespace VillageGame.Infrastructure.Factories
{
    public class UIFactory
    {
        private readonly UIFacade _uiFacade;

        [Inject]
        public UIFactory(IObjectResolver resolver)
        {
            _uiFacade = resolver.Resolve<UIFacade>();
        }

        public ShopBuildingUIButton CreateBuildingButton(ShopBuildingUIButton.Data buttonData)
        {
            var panel = _uiFacade.FindPanel<BuildShopUIPanel>();
        
            if (panel.TryGetCreatedButton(buttonData.BuildingType,buttonData.ID, out var button))
            {
                button.Show();
            }
            else
            {
                var instance = Object.Instantiate(_uiFacade.BuildingButtonPrefab, panel.ContentTransform);
                button = instance.GetComponent<ShopBuildingUIButton>();
                panel.AddButton(button);
            }

            button.Initialize(panel,buttonData);
            return button;
        }

        public CalendarPresentButton CreatePresentButton(bool isLeft)
        {
            var panel = _uiFacade.FindPanel<CalendarPanel>();
            return  Object.Instantiate(
                isLeft ? 
                    _uiFacade.CalendarPresentButtonPrefabLeft :
                    _uiFacade.CalendarPresentButtonPrefabRight,
                panel.ContentTransform);
        }

        public DailyTaskButton CreateDailyTaskButton()
        {
            var panel = _uiFacade.FindPanel<DailyTasksPanel>();
            return Object.Instantiate(_uiFacade.DailyTaskButtonPrefab, panel.ContentTransform);
        }
        
        public DailyVisitButton CreateDailyVisitButton()
        {
            var panel = _uiFacade.FindPanel<DailyVisitPanel>();
            return Object.Instantiate(_uiFacade.DailyVisitButtonPrefab, panel.ContentTransform);
        }

        /*public NewYearProjectSubPanel CreateNewYearProjectSubpanel()
        {
            var panel = _uiFacade.FindPanel<NewYearProjectsPanel>();
            return Object.Instantiate(_uiFacade.NewYearProjectSubpanelPrefab, panel.ContentTransform);
        }*/
    }
}