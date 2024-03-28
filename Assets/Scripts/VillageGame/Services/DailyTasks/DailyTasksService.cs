using System;
using System.Collections.Generic;
using System.Linq;
using CoreGame;
using SO;
using SO.Data;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;
using Web.RequestStructs;
using Web.ResponseStructs;

namespace VillageGame.Services.DailyTasks
{
    public partial class DailyTasksService : IInitializable, ILoading
    {
        private readonly DailyTasksConfig _dailyTaskConfig;
        private readonly ConditionProvider _conditionsProvider;
        private readonly ProgressionService _progressionService;
        private readonly RatingStorage _ratingStorage;
        private readonly TimeObserver _timeObserver;
        private readonly WinCounter _winCounter;
        private readonly ConstructionSiteService _constructionSiteService;

        public TodayTasksState TodayTasksState { get; private set; } = new();
        public List<DailyTaskData> AllTodayTasks { get; private set; } = new();

        public Action<DailyTaskData> CompliteTaskEvent;
        public Action<DailyTaskData> CloseTaskEvent;
        public Action<TodayTasksState> SetNewTasksListEvent;

        private List<string> CompleteTrackingTasks = new();

        [Inject]
        public DailyTasksService(IObjectResolver objectResolver)
        {
            _dailyTaskConfig = objectResolver.Resolve<DailyTasksConfig>();
            _conditionsProvider = objectResolver.Resolve<ConditionProvider>();
            _progressionService = objectResolver.Resolve<ProgressionService>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            _winCounter = objectResolver.Resolve<WinCounter>();
            _constructionSiteService = objectResolver.Resolve<ConstructionSiteService>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~DailyTasksService()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _conditionsProvider.CreateNewConditionEvent += CheckNewCondition;
                _timeObserver.StartNewDayInGameEvent += RefreshList;

                RealLifeService.TaskDone += RealLifeTaskDone;
            }
            else
            {
                _conditionsProvider.CreateNewConditionEvent -= CheckNewCondition;
                _timeObserver.StartNewDayInGameEvent -= RefreshList;

                RealLifeService.TaskDone -= RealLifeTaskDone;
            }
        }

        private void RefreshList()
        {
            InitDailyTasks();
            InitTasksState();
        }

        private void InitDailyTasks()
        {
            AllTodayTasks?.Clear();
            AllTodayTasks = new List<DailyTaskData>();

            TryAddRealLifeTask();
            TryAddBuildingTask();
            TryAddDecorationTask();
            TryAddSnowdriftTask();
            TryAddLevelUpTask();
            TryAddCoreGameWinLevelTask();
            TryAddRandomTasks();
        }

        private void InitTasksState()
        {
            TodayTasksState.tasks = new List<DailyTask>();
            foreach (var todayTask in AllTodayTasks)
            {
                var newTask = new DailyTask
                {
                    condition = todayTask.Condition,
                    state = TaskState.in_progress
                };
                TodayTasksState.tasks.Add(newTask);
            }
        }

        public void GetTaskProgress(ConditionData condition, out int current, out int max)
        {
            current = 0;
            max = 1;

            switch (condition.Type)
            {
                case ConditionType.LevelUp:
                    current = _ratingStorage.Level;
                    max = condition.Value;
                    break;
                case ConditionType.CoreGameNumberWinInARow:
                    current = _winCounter.WinNumberData.wins_in_row;
                    max = condition.Value;
                    break;
                case ConditionType.CoreGameNumberWin:
                    current = _winCounter.WinNumberData.all_wins;
                    max = condition.Value;
                    break;
                case ConditionType.ClearSnowdrift:
                    var amount = TodayTasksState.snowdrift_amount - _constructionSiteService.GetSnowdriftCount();
                    max = condition.Value;
                    current = Mathf.Clamp(amount,0,max);
                    break;
            }
        }

        private void CheckNewCondition(ConditionData condition)
        {
            if (condition.Type == ConditionType.ClearSnowdrift)
            {
                condition.Value = TodayTasksState.snowdrift_amount - condition.Value;
            }
            
            if (!IsHasThisTasks(condition, out var task) || GetTaskState(condition) is TaskState.none)
            {
                return;
            }

            if (GetTaskState(condition) is TaskState.in_progress)
            {
                var dayTask = TodayTasksState.tasks.FirstOrDefault(t => t.condition.Equals(task.Condition));
                
                if (dayTask != null)
                {
                    dayTask.state = TaskState.complited;
                    CompliteTaskEvent?.Invoke(task);
                }
            }
        }

        private void RealLifeTaskDone(int taskID)
        {
            var realTask = TodayTasksState.tasks.FirstOrDefault(t => _dailyTaskConfig.GetRealLifeTaskID(t) == taskID);
            if (realTask == null)
            {
                Debugging.Log($"Real life task with ID:{taskID} not found!");
                return;
            }

            if (!IsHasThisTasks(realTask.condition, out var task))
            {
                Debugging.Log("Something went wrong!", ColorType.Red);
                return;
            }

            realTask.state = TaskState.complited;
            CompliteTaskEvent?.Invoke(task);
        }

        public void CloseTask(ConditionData condition)
        {
            var stateData = TodayTasksState.tasks.FirstOrDefault(t => t.condition.Equals(condition));

            if (stateData != null)
            {
                DodoAnalyticsService.SendTaskDone(condition);
                stateData.state = TaskState.close;
                var taskData = AllTodayTasks.FirstOrDefault(t => t.Condition.Equals(condition));
                CloseTaskEvent?.Invoke(taskData);
                SetNewTasksListEvent?.Invoke(TodayTasksState);
            }
        }


        public void Load(LoadData request)
        {
            if (request.data.daily_tasks == null
                || request.data.daily_tasks.tasks.Count == 0
                || request.data.daily_tasks.date != _timeObserver.CurrentDay.GetFullDateText())
            {
                CompleteTrackingTasks = request.data.complete_tracking_tasks ?? new List<string>();
                InitDailyTasks();
                InitTasksState();
                TodayTasksState.date = _timeObserver.CurrentDay.GetFullDateText();
                TodayTasksState.snowdrift_amount = _constructionSiteService.GetSnowdriftCount();
                SetNewTasksListEvent?.Invoke(TodayTasksState);
            }
            else
            {
                CompleteTrackingTasks = request.data.complete_tracking_tasks ?? new List<string>();
                var state = request.data.daily_tasks;
                foreach (var task in state.tasks)
                {
                    if (task.condition.Type is ConditionType.OrderPrice or ConditionType.BuyProduct)
                    {
                        var id = _dailyTaskConfig.GetRealLifeTaskID(task);
                        if (CompleteTrackingTasks.Contains(id.ToString()))
                        {
                            if (task.state == TaskState.in_progress)
                                task.state = TaskState.complited;
                        }
                    }
                }

                TodayTasksState = state;
                AllTodayTasks = _dailyTaskConfig.GetTasks(TodayTasksState);
            }
        }
    }
}