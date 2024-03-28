using UnityEngine;
using VillageGame.Data.Types;
using VillageGame.UI.Buttons;
using VillageGame.UI.Indicators;

namespace VillageGame.UI.Panels
{
    public class MainUIPanel : UIPanel
    {
        [SerializeField] private LevelUIIndicator _levelUIIndicator;
        [SerializeField] private CurrencyUIIndicator _currencyUIIndicator;
        [SerializeField] private EnergyUIIndicator _energyUIIndicator;

        [Space, Header("Buttons")] 
        public MainUIButton CoreGameButton;
        public MainUIButton BuildingShopButton;
        public MainUIButton CalendarButton;
        public MainUIButton DailyTasksButton;
        //public MainUIButton NewYearButton;
        
        public MainUIButton GetButtonByType(WindowType windowType)
        {
            switch (windowType)
            {
                case WindowType.None:
                default:
                    return null;
                case WindowType.BuildingShop:
                    return BuildingShopButton;
                case WindowType.DailyTasks:
                    return DailyTasksButton;
                case WindowType.NewYearProject:
                    return null;
                case WindowType.Calendar:
                    return CalendarButton;
            }
        }
        public MainUIButton[] GetPanelButtonsByTutorialId(int tutorialID)
        {
            return tutorialID switch
            {
                0 => new[] { CoreGameButton },
                1 => new[] { CalendarButton },
                3 => new[] { BuildingShopButton },
                4 => new[] { DailyTasksButton },
                _ => null
            };
        }

        public void UpdateEnergyValue(int value)
        {
            _energyUIIndicator.UpdateValue(value.ToString());
        }

        public void SetNewYearProjectsButton(bool isActive)
        {
            //Новогодние проекты было решено вообще отрубить до лучших времен
            //Кнопка вообще не появляется, пока стоит это условие
            if (!isActive)
            {
                //NewYearButton.gameObject.SetActive(isActive);
            }
        }

        #region Test

        public void UpdateLevelValue(int value)
        {
            _levelUIIndicator.UpdateValue(value.ToString());
        }

        public void UpdateCurrencyValue(int value)
        {
            _currencyUIIndicator.UpdateValue(value.ToString());
        }

        public void UpdateRatingValue(int current, int max)
        {
            _levelUIIndicator.UpdateExp(current, max);
        }

        #endregion
    }
}