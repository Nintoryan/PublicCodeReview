using SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VillageGame.Data.Types;
using VillageGame.Logic.Tree;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Storages;
using VillageGame.UI.Panels;
using Web.RequestStructs;

namespace VillageGame.Services
{
    public class NewYearProjectsService : IInitializable, ILoading
    {
        private readonly NewYearProjectsDatabase _newYearProjectsDatabase;
        private readonly BuildingOnMapStorage _buildingsOnMapStorage;
        private readonly ProgressionService _progressionService;
        private readonly ChristmasTree _christmasTree;
        private readonly CurrencyStorage _currencyStorage;

        private bool _isActivated;
        private List<int> _newYearProjectsStates;

        public Action NewYearProjectsUnlockedEvent;
        public Action NewYearProjectsActivatedEvent;
        public Action<List<int>> NewYearProjectsUpdatedEvent;
        public Action<List<NewYearProjectPresentation>> NewYearProjectPresentationsUpdatedEvent;

        public Action<bool> LanternProjectSettedEvent;

        [Inject]
        public NewYearProjectsService(IObjectResolver resolver)
        {
            _newYearProjectsDatabase = resolver.Resolve<NewYearProjectsDatabase>();
            _buildingsOnMapStorage = resolver.Resolve<BuildingOnMapStorage>();
            _progressionService = resolver.Resolve<ProgressionService>();
            _christmasTree = resolver.Resolve<ChristmasTree>();
            _currencyStorage = resolver.Resolve<CurrencyStorage>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~NewYearProjectsService()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _buildingsOnMapStorage.BuildBuildingEvent += OnBuildBuildingEvent;
            }
            else
            {
                _buildingsOnMapStorage.BuildBuildingEvent -= OnBuildBuildingEvent;
            }
        }

        private void OnBuildBuildingEvent(BuildingType type, int index)
        {
            if (!_isActivated)
            {
                if (type == BuildingType.House && index == 8)
                {
                    NewYearProjectsUnlockedEvent?.Invoke();
                    NewYearProjectsActivatedEvent?.Invoke();
                    _isActivated = true;
                }
            }
        }

        public void UpdateProjectStates()
        {
            List<int> unlockedProjects = _progressionService.GetUnlockedNewYearProjects();

            List<NewYearProjectPresentation> newYearProjectPresentations = new();
            for (int i = 0; i < _newYearProjectsStates.Count; i++)
            {
                ProjectState currentState = (ProjectState)_newYearProjectsStates[i];
                if (currentState == ProjectState.Locked && unlockedProjects.Any(x => x == i))
                {
                    currentState = ProjectState.Unlocked;
                }
                NewYearProjectPresentation newYearProjectPresentation = new(
                    _newYearProjectsDatabase.NewYearProjects[i].Name,
                    i,
                    _newYearProjectsDatabase.NewYearProjects[i].Icon,
                    currentState,
                    _newYearProjectsDatabase.NewYearProjects[i].Cost,
                    _newYearProjectsDatabase.NewYearProjects[i].ProjectReward,
                    _newYearProjectsDatabase.NewYearProjects[i].Description
                    );
                newYearProjectPresentations.Add(newYearProjectPresentation);
            }
            NewYearProjectPresentationsUpdatedEvent?.Invoke(newYearProjectPresentations);
        }

        public void PurchaseProject(int id)
        {
            int cost = _newYearProjectsDatabase.NewYearProjects[id].Cost;
            if (_currencyStorage.IsEnoughCurrency(cost))
            {
                _currencyStorage.Remove(cost);
                _newYearProjectsStates[id] = (int)ProjectState.Active;
                UpdateProjectStates();
                _christmasTree.AddValueToBank(_newYearProjectsDatabase.NewYearProjects[id].ProjectReward);
                NewYearProjectsUpdatedEvent?.Invoke(_newYearProjectsStates);
                ActivateProjectMechanic(id);
            }
        }

        public void ActivateProject(int id)
        {
            if (_newYearProjectsStates[id] == (int)ProjectState.Inactive)
            {
                _newYearProjectsStates[id] = (int)ProjectState.Active;
                NewYearProjectsUpdatedEvent?.Invoke(_newYearProjectsStates);
                ActivateProjectMechanic(id);
            }
        }

        public void DeactivateProject(int id)
        {
            if (_newYearProjectsStates[id] == (int)ProjectState.Active)
            {
                _newYearProjectsStates[id] = (int)ProjectState.Inactive;
                NewYearProjectsUpdatedEvent?.Invoke(_newYearProjectsStates);
                DeactivateProjectMechanic(id);
            }
        }

        private void ActivateProjectMechanic(int id)
        {
            Debug.LogWarning("Activate Project " + id);
            switch (id)
            {
                case 0:
                    LanternProjectSettedEvent?.Invoke(true);
                    break;
                case 1:
                    break;
                    //И т.д.
            }
        }

        private void DeactivateProjectMechanic(int id)
        {
            Debug.LogWarning("Deactivate Project " + id);
            switch (id)
            {
                case 0:
                    LanternProjectSettedEvent?.Invoke(false);
                    break;
                case 1:
                    break;
                    //И т.д.
            }
        }

        public void Load(LoadData request)
        {
            List<int> projectsStates = new List<int>();
            bool isReset = request.data.new_year_projects_states == null || request.data.new_year_projects_states.Count == 0;

            for (int i = 0; i < _newYearProjectsDatabase.NewYearProjects.Count; i++)
            {
                if (isReset)
                {
                    projectsStates.Add(1);
                }
                else
                {
                    int state = request.data.new_year_projects_states[i];
                    projectsStates.Add(state);
                    if ((ProjectState)state == ProjectState.Active)
                    {
                        ActivateProjectMechanic(i);
                    }
                }
            }

            _newYearProjectsStates = projectsStates;
            if (isReset)
            {
                NewYearProjectsUpdatedEvent?.Invoke(_newYearProjectsStates);
            }

            if (request.data.new_year_projects_unlocked)
            {
                _isActivated = true;
                NewYearProjectsActivatedEvent?.Invoke();
            }
        }
    }

    [Serializable]
    public class NewYearProjectsData
    {
        public List<ProjectData> projects;

        public NewYearProjectsData(List<ProjectData> projects)
        {
            this.projects = projects;
        }
    }

    [Serializable]
    public class ProjectData
    {
        public int state_id;

        public ProjectData(int state_id)
        {
            this.state_id = state_id;
        }
    }

    [Serializable]
    public class NewYearProjectPresentation
    {
        public string ProjectName;
        public int ID;
        public Sprite Icon;
        public ProjectState State;
        public int Cost;
        public int Reward;
        public string Description;

        public NewYearProjectPresentation(string projectName, int iD, Sprite icon, ProjectState state, int cost, int reward, string description)
        {
            ProjectName = projectName;
            ID = iD;
            Icon = icon;
            State = state;
            Cost = cost;
            Reward = reward;
            Description = description;
        }
    }
}
