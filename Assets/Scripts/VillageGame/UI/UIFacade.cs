using System.Collections.Generic;
using Tutorial;
using UnityEngine;
using VillageGame.Logic.Curtains;
using VillageGame.Logic.Tutorial;
using VillageGame.Services.CutScenes;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;

namespace VillageGame.UI
{
    public class UIFacade : MonoBehaviour
    {
        [SerializeField] private CloudCurtain _cloudCurtain;
        [Header("Panels")]
        [SerializeField] private List<UIPanel> _panels;
        [SerializeField] private List<WarningPanel> _warningPanels;
        [Header("Tutorial")]
        [SerializeField] private TutorialBlackScreen _tutorialBlackScreen;
        [SerializeField] private TutorialFinger _tutorialFinger;
        [SerializeField] private TutorialCanvas _tutorialCanvas;
        [SerializeField] private TutorialMansFacade _tutorialManFacade;
        [Header("Temp")]
        [SerializeField] private GameObject _buildingButtonPrefab;
        [SerializeField] private CalendarPresentButton _presenetButtonPrefabLeft;
        [SerializeField] private CalendarPresentButton _presenetButtonPrefabRight;
        [SerializeField] private DailyTaskButton _dailyTaskButtonPrefab;
        [SerializeField] private DailyVisitButton _dailyVisitButtonPrefab;
        [SerializeField] private NewYearProjectSubPanel _newYearProjectSubpanelPrefabLeft;
        [SerializeField] private NewYearProjectSubPanel _newYearProjectSubpanelPrefabRight;
        [SerializeField] private CalendarShopButton _leftShopButton;
        [SerializeField] private CalendarShopButton _rightShopButton;

        public TutorialMansFacade TutorialManFacade => _tutorialManFacade;
        public TutorialCanvas TutorialCanvas => _tutorialCanvas;
        public TutorialFinger TutorialFinger => _tutorialFinger;
        public TutorialBlackScreen TutorialBlackScreen => _tutorialBlackScreen;
        public CloudCurtain CloudCurtain => _cloudCurtain;
        public List<UIPanel> Panels => _panels;
        public GameObject BuildingButtonPrefab => _buildingButtonPrefab;
        public CalendarPresentButton CalendarPresentButtonPrefabLeft => _presenetButtonPrefabLeft;
        public CalendarPresentButton CalendarPresentButtonPrefabRight => _presenetButtonPrefabRight;
        public DailyTaskButton DailyTaskButtonPrefab => _dailyTaskButtonPrefab;
        public DailyVisitButton DailyVisitButtonPrefab => _dailyVisitButtonPrefab;

        public NewYearProjectSubPanel NewYearProjectSubpanelPrefabLeft => _newYearProjectSubpanelPrefabLeft;
        public NewYearProjectSubPanel NewYearProjectSubpanelPrefabRight => _newYearProjectSubpanelPrefabRight;

        public CalendarShopButton LeftShopButton => _leftShopButton;
        public CalendarShopButton RightShopButton => _rightShopButton;

        public T FindPanel<T>() where  T : UIPanel
        {
            foreach (var panel in Panels)
            {
                if (panel is T uiPanel)
                {
                    return uiPanel;
                }
            }
            return null;
        }
        
        public T FindWarningPanel<T>() where T : WarningPanel
        {
            foreach (var panel in _warningPanels)
            {
                if (panel is T uiPanel)
                {
                    return uiPanel;
                }
            }
            return null;
        }
    }
}