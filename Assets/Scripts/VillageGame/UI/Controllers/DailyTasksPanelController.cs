using System.Collections.Generic;
using System.Linq;
using SO;
using SO.Data;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data.Types;
using VillageGame.Infrastructure.Factories;
using VillageGame.Services;
using VillageGame.Services.DailyTasks;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;
using Web.ResponseStructs;
using MainUIPanel = VillageGame.UI.Panels.MainUIPanel;

namespace VillageGame.UI.Controllers
{
    public class DailyTasksPanelController : IInitializable, ITickable
    {
        private readonly UIFactory _uiFactory;
        private readonly DailyTasksPanel _dailyTasksPanel;
        private readonly MainUIPanelController _mainPanelController;
        private readonly ApplicationObserver _applicationObserver;
        private readonly DailyTasksService _dailyTasksService;
        private readonly BuildingsConfig _buildingsConfig;

        private readonly MainUIPanel _mainPanel;

        private readonly List<DailyTaskButton> _buttons = new();
        private readonly TimeObserver _timeObserver;

        [Inject]
        public DailyTasksPanelController(IObjectResolver objectResolver)
        {
            _mainPanel = objectResolver.Resolve<UIFacade>().FindPanel<MainUIPanel>();
            _dailyTasksPanel = objectResolver.Resolve<UIFacade>().FindPanel<DailyTasksPanel>();
            _uiFactory = objectResolver.Resolve<UIFactory>();
            _buildingsConfig = objectResolver.Resolve<BuildingsConfig>();
            _applicationObserver = objectResolver.Resolve<ApplicationObserver>();
            _dailyTasksService = objectResolver.Resolve<DailyTasksService>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~DailyTasksPanelController()
        {
            SubscribeToEvents(false);
            UnSubscribeFromButtonsEvents();
        }

        public void Tick()
        {
            if (_dailyTasksPanel.IsActive)
            {
                UpdateTime();
            }
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _mainPanel.DailyTasksButton.ClickEvent += SwitchDailyTasksEvent;
                _applicationObserver.SceneLoadEvent += SceneLoadEvent;
                _dailyTasksService.CompliteTaskEvent += CompletedTaskEvent;
                _dailyTasksPanel.PanelSwitchEvent += OnPanelSwitch;
            }
            else
            {
                _mainPanel.DailyTasksButton.ClickEvent -= SwitchDailyTasksEvent;
                _applicationObserver.SceneLoadEvent -= SceneLoadEvent;
                _dailyTasksService.CompliteTaskEvent -= CompletedTaskEvent;
                _dailyTasksPanel.PanelSwitchEvent -= OnPanelSwitch;
            }
        }

        private void OnPanelSwitch(UIPanel panel, bool isOpen)
        {
            if (panel != _dailyTasksPanel || !isOpen) return;

            foreach (var button in _buttons)
            {
                if (button.State == TaskState.in_progress)
                {
                    UpdateButtonProgress(button);
                }
            }
        }

        private void UpdateButtonProgress(DailyTaskButton button)
        {
            //todo раскоменить когда разберемся с url
            if (button != null /*&& button.Condition.Type != ConditionType.BuyProduct*/ )
            {
                _dailyTasksService.GetTaskProgress(button.Condition, out var current, out var max);
                button.SetInProgressMode(current, max);
            }
        }

        private void CompletedTaskEvent(DailyTaskData taskData)
        {
            var button = _buttons.FirstOrDefault(b => b.Condition.Equals(taskData.Condition));
            Debugging.Log($"DailyTasksPanelController -> CompletedTaskEvent {button != null}");
            if (button != null)
            {
                button.SetCompleteMode(taskData.Condition.Value);
                button.transform.SetAsFirstSibling();
            }
        }

        private void SceneLoadEvent()
        {
            foreach (var taskData in _dailyTasksService.AllTodayTasks)
            {
                if (taskData == null)
                {
                    Debugging.Log($"DailyTasksPanelController: SceneLoadEvent -> task data == null", LogStyle.Warning);
                    continue;
                }

                var taskButton = _uiFactory.CreateDailyTaskButton();
                taskButton.SetCondition(taskData.Condition);
                taskButton.SetDescription(taskData.Desctiption);
                taskButton.SetIcon(taskData.Icon);
                taskButton.SetCurrencyReward(taskData.CurrencyReward);
                taskButton.SetRatingReward(taskData.RatingReward);
                taskButton.SetShufflesReward(taskData.ShuffleReward);
                taskButton.SetHintsReward(taskData.HintReward);

                var data = _buildingsConfig.Decorations.FirstOrDefault(d => d.ID == taskData.DecorationRewardID);

                if (data != null)
                {
                    taskButton.SetDecoration(taskData.DecorationRewardID, data.PresentationModel.Icon);
                }
                else
                {
                    taskButton.SetDecoration(-1);
                }

                var taskState = _dailyTasksService.GetTaskState(taskData.Condition);
                _dailyTasksService.GetTaskProgress(taskData.Condition, out var current, out var max);

                if (current >= max && taskState == TaskState.in_progress)
                {
                    taskState = TaskState.complited;
                }

                switch (taskState)
                {
                    case TaskState.in_progress:
                        //todo раскоменить когда разберемся с url
                        /*if (taskData.Condition.Type == ConditionType.BuyProduct)
                        {
                            var link = $"dodo://product/{taskData.Condition.ValueString}_M";
                            taskButton.SetDeepLinkMode(current, link);
                            Debugging.Log($"task type {taskData.Condition.Type}  set deep link mode\n{taskData.Desctiption}");
                        }
                        else
                        {
                            taskButton.SetInProgressMode(current, max);
                            Debugging.Log($"task type {taskData.Condition.Type} set progress mode");
                        }*/

                        //временно пока нет url
                        taskButton.SetInProgressMode(current, max);
                        Debugging.Log($"task type {taskData.Condition.Type} set progress mode");
                        //
                        taskButton.PressEvent += OnPressDailyTaskButton;
                        break;
                    case TaskState.complited:
                        taskButton.SetCompleteMode(taskData.Condition.Value);
                        taskButton.transform.SetAsFirstSibling();
                        taskButton.PressEvent += OnPressDailyTaskButton;
                        break;
                    case TaskState.close:
                        taskButton.SetCloseMode(max);
                        taskButton.transform.SetAsLastSibling();
                        break;
                    case TaskState.none:
                    default:
                        break;
                }

                _buttons.Add(taskButton);
            }

            foreach (var taskButton in _buttons)
            {
                if (taskButton.Condition.Type is ConditionType.OrderPrice or ConditionType.BuyProduct)
                {
                    taskButton.transform.SetAsFirstSibling();
                }
            }

            foreach (var taskButton in _buttons)
            {
                if (taskButton.State == TaskState.close)
                {
                    taskButton.transform.SetAsLastSibling();
                }
            }
        }

        private void OnPressDailyTaskButton(DailyTaskButton dailyTaskButton)
        {
            if (dailyTaskButton.State == TaskState.complited)
            {
                _dailyTasksService.GetTaskProgress(dailyTaskButton.Condition, out _, out var max);

                dailyTaskButton.PressEvent -= OnPressDailyTaskButton;
                dailyTaskButton.SetCloseMode(max);
                dailyTaskButton.transform.SetAsLastSibling();
                _dailyTasksService.CloseTask(dailyTaskButton.Condition);
            }
        }

        private void UpdateTime()
        {
            _dailyTasksPanel.SetTimerValue(
                $"{_timeObserver.RemainingTime.Hours:00}:{_timeObserver.RemainingTime.Minutes:00}");
        }

        private void SwitchDailyTasksEvent()
        {
            _dailyTasksPanel.Switch();
        }

        private void UnSubscribeFromButtonsEvents()
        {
            foreach (var button in _buttons)
            {
                button.PressEvent -= OnPressDailyTaskButton;
            }
        }
    }
}