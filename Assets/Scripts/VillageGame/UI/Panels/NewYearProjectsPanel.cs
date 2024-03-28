using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VContainer;
using VillageGame.Services;
using VillageGame.Services.Storages;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class NewYearProjectsPanel : UIPanel
    {
        [SerializeField] private UIFacade _uIFacade;

        [SerializeField] private MainUIButton _newYearProjectsPanelButton;
        [SerializeField] private Transform contentTransform;
        [SerializeField] private NewYearSelectedProjectPanel _newYearSelectedProjectPanel;
        [SerializeField] private NewYearProjectPurchaseButton _newYearProjectPurchaseButton;
        [SerializeField] private NewYearProjectsTabButton _newProjectsTabButton;
        [SerializeField] private NewYearProjectsTabButton _completedProjectsTabButton;

        private CurrencyStorage _currencyStorage;
        private NewYearProjectsService _newYearProjectsService;
        private bool _isCompletedTabActive = false;
        private List<NewYearProjectPresentation> _newYearProjectPresentations;
        private int _selectedProject = -1;

        private List<NewYearProjectSubPanel> newYearProjectSubPanels = new();
        public Transform ContentTransform => contentTransform;

        public event Action<bool> TabSwitched; 

        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            _newYearProjectsService = resolver.Resolve<NewYearProjectsService>();
            _uIFacade = resolver.Resolve<UIFacade>();
            _currencyStorage = resolver.Resolve<CurrencyStorage>();
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _newYearProjectsPanelButton.ClickEvent += OnPanelButtonClickEvent;
                _newYearProjectsService.NewYearProjectPresentationsUpdatedEvent += OnNewYearProjectsUpdatedEvent;
            }
            else
            {
                _newYearProjectsPanelButton.ClickEvent -= OnPanelButtonClickEvent;
                _newYearProjectsService.NewYearProjectPresentationsUpdatedEvent -= OnNewYearProjectsUpdatedEvent;
            }
        }

        public void LaunchProject(int projectId)
        {
            _selectedProject = projectId;
            _newYearProjectPurchaseButton.SetCostText(_newYearProjectPresentations[_selectedProject].Cost.ToString());
            if (!_currencyStorage.IsEnoughCurrency(_newYearProjectPresentations[_selectedProject].Cost))
            {
                _newYearProjectPurchaseButton.SetInsufficientCurrency();
            }
            _newYearSelectedProjectPanel.Initialize(
                _newYearProjectPresentations[_selectedProject].Icon,
                _newYearProjectPresentations[_selectedProject].ProjectName,
                _newYearProjectPresentations[_selectedProject].Description, 
                _newYearProjectPresentations[_selectedProject].Reward.ToString()
                );
            body.DOAnchorPosX(-2000, 0.5f).SetEase(Ease.OutBack);
        }

        public void PurchaseProject()
        {
            _newYearProjectsService.PurchaseProject(_selectedProject);
        }

        public void ActivateProject(int projectId)
        {
            _newYearProjectsService.ActivateProject(projectId);
        }

        public void DeactivateProject(int projectId)
        {
            _newYearProjectsService.DeactivateProject(projectId);
        }

        private void OnNewYearProjectsUpdatedEvent(List<NewYearProjectPresentation> newYearProjectPresentations)
        {
            _newYearProjectPresentations = newYearProjectPresentations;

            foreach (var subPanel in newYearProjectSubPanels)
            {
                Destroy(subPanel.gameObject);
            }
            newYearProjectSubPanels.Clear();

            for (int i = 0; i < newYearProjectPresentations.Count; i++)
            {
                bool isShown = false;
                if(_isCompletedTabActive)
                {
                    if (newYearProjectPresentations[i].State == ProjectState.Active || newYearProjectPresentations[i].State == ProjectState.Inactive)
                    {
                        isShown = true;
                    }
                }
                else
                {
                    if (newYearProjectPresentations[i].State == ProjectState.Locked || newYearProjectPresentations[i].State == ProjectState.Unlocked)
                    {
                        isShown = true;
                    }
                }
                if (isShown)
                {
                    var prefab = i % 2 == 0
                        ? _uIFacade.NewYearProjectSubpanelPrefabLeft
                        : _uIFacade.NewYearProjectSubpanelPrefabRight;
                    
                    var instance = Instantiate(prefab, contentTransform);
                    instance.Initialize(newYearProjectPresentations[i], this);
                    newYearProjectSubPanels.Add(instance);
                }
            }
        }

        private void OnPanelButtonClickEvent()
        {
            Switch();
            _newProjectsTabButton.UpdateTab(_isCompletedTabActive);
            _completedProjectsTabButton.UpdateTab(_isCompletedTabActive);
            _newYearProjectsService.UpdateProjectStates();
        }

        public void OnTabButtonClick(bool isCompleted)
        {
            _isCompletedTabActive = isCompleted;
            TabSwitched?.Invoke(!_isCompletedTabActive);
            _newProjectsTabButton.UpdateTab(isCompleted);
            _completedProjectsTabButton.UpdateTab(isCompleted);
            _newYearProjectsService.UpdateProjectStates();
        }
    }
}
