using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SO.Data;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Services.DailyTasks;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Presents;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Panels;
using VillageGame.UI.Services;
using Web.RequestStructs;

namespace VillageGame.Services
{
    public class MainPanelNotificationController : IInitializable, ILoading
    {
        private readonly MainUIPanel _mainUIPanel;
        private readonly ProgressionService _progressionService;
        private readonly PurchasedBuildingsStorage _purchasedBuildingsStorage;
        private readonly DailyTasksService _dailyTasksService;

        private List<NotificationStateData> _currentData;
        private readonly CalendarPresentsStorage _calendarPresentStorage;

        [Inject]
        public MainPanelNotificationController(IObjectResolver objectResolver)
        {
            _mainUIPanel = objectResolver.Resolve<UIFacade>().FindPanel<MainUIPanel>();
            _progressionService = objectResolver.Resolve<ProgressionService>();
            _purchasedBuildingsStorage = objectResolver.Resolve<PurchasedBuildingsStorage>();
            _dailyTasksService = objectResolver.Resolve<DailyTasksService>();
            _calendarPresentStorage = objectResolver.Resolve<CalendarPresentsStorage>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~MainPanelNotificationController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                UIWindowObserver.SwitchWindowEvent += OnSwitchWindow;
                _dailyTasksService.CompliteTaskEvent += OnCompliteTaskEvent;
                _progressionService.NewBuildingUnlockedEvent += OnNewBuildingUnlocked;
                _progressionService.NewProjectUnlockedEvent += OnNewProjectUnlocked;
                _calendarPresentStorage.TakeNewClosedPresentBoxEvent += TakeNewClosedPresentBoxEvent;
                _purchasedBuildingsStorage.BuildingHasBeenAddedAsPresentEvent += OnBuildingHasBeenAddedAsPresent;
            }
            else
            {
                UIWindowObserver.SwitchWindowEvent -= OnSwitchWindow;
                _dailyTasksService.CompliteTaskEvent -= OnCompliteTaskEvent;
                _progressionService.NewBuildingUnlockedEvent -= OnNewBuildingUnlocked;
                _progressionService.NewProjectUnlockedEvent -= OnNewProjectUnlocked;
                _calendarPresentStorage.TakeNewClosedPresentBoxEvent -= TakeNewClosedPresentBoxEvent;
                _purchasedBuildingsStorage.BuildingHasBeenAddedAsPresentEvent -= OnBuildingHasBeenAddedAsPresent;
            }
        }

        private void TakeNewClosedPresentBoxEvent(PresentBoxData obj)
        {
            ShowNotification(WindowType.Calendar, withAnimation: true);
        }

        private void OnCompliteTaskEvent(DailyTaskData _)
        {
            ShowNotification(WindowType.DailyTasks, withAnimation: true);
        }

        private void OnBuildingHasBeenAddedAsPresent()
        {
            ShowNotification(WindowType.BuildingShop, withAnimation: true);
        }

        private void OnNewProjectUnlocked()
        {
            ShowNotification(WindowType.NewYearProject, withAnimation: true);
        }

        private void OnNewBuildingUnlocked()
        {
            ShowNotification(WindowType.BuildingShop, withAnimation: true);
        }

        private void OnSwitchWindow(WindowType windowType, bool isOpen)
        {
            var data = _currentData.FirstOrDefault(p => p.Type == windowType);
            if (data is { IsHasAttention: true })
            {
                data.IsHasAttention = false;
                _mainUIPanel.GetButtonByType(windowType)?.HideNotification(isAnimation: true);
            }
        }

        private void ShowNotification(WindowType windowType, bool withAnimation)
        {
            var data = _currentData.FirstOrDefault(p => p.Type == windowType);
            if (data !=null)
            {
                data.IsHasAttention = true;
                _mainUIPanel.GetButtonByType(windowType)?.ShowNotification(withAnimation);
            }
        }

        private void HideNotification(WindowType windowType, bool withAnimation)
        {
            var data = _currentData.FirstOrDefault(p => p.Type == windowType);
            if (data != null)
            {
                data.IsHasAttention = false;
                _mainUIPanel.GetButtonByType(windowType)?.HideNotification(withAnimation);
            }
        }

        public void Load(LoadData request)
        {
            if (request.data.notification == null)
            {
                _currentData = new List<NotificationStateData>()
                {
                    new() { Type = WindowType.DailyTasks },
                    new() { Type = WindowType.BuildingShop },
                    new() { Type = WindowType.NewYearProject },
                    new() { Type = WindowType.Calendar },
                };
            }
            else
            {
                _currentData = request.data.notification;
            }

            foreach (var notificationData in _currentData)
            {
                if (notificationData.IsHasAttention)
                {
                    ShowNotification(notificationData.Type, withAnimation: false);
                }
                else
                {
                    HideNotification(notificationData.Type, withAnimation: false);
                }
            }
        }
    }
}