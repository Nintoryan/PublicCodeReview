using System.Collections.Generic;
using DefaultNamespace.VillageGame.Infrastructure;
using SO;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Logic.Curtains;
using VillageGame.Logic.Tutorial;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Services
{
    public class MainButtonsController : IInitializable, ITickable
    {
        private const float ShowButtonsDelay = 0.7f;
        
        private readonly MainUIPanel _mainPanel;
        private readonly TutorialStateStorage _tutorialStateStorage;
        private readonly List<MainUIButton> _unlockedButtons = new();
        private readonly CutSceneConfig _cutSceneConfig;
        private readonly NewYearProjectsService _newYearService;
        private readonly CutSceneStartController _cutsceneStartController;

        private float _showButtonsTimer;

        [Inject]
        public MainButtonsController(IObjectResolver resolver)
        {
            _tutorialStateStorage = resolver.Resolve<TutorialStateStorage>();
            _mainPanel = resolver.Resolve<UIFacade>().FindPanel<MainUIPanel>();
            _cutSceneConfig = resolver.Resolve<CutSceneConfig>();
            _newYearService = resolver.Resolve<NewYearProjectsService>();
            _cutsceneStartController = resolver.Resolve<CutSceneStartController>();
            
            _newYearService.NewYearProjectsActivatedEvent += OnNewYear;
            GlobalEvent.LoadGameEvent += LoadTutorialButtonsState;
            _tutorialStateStorage.StartWatchTutorialEvent += AddTutorialButton;
            
            
            _showButtonsTimer = ShowButtonsDelay;
        }

        private void OnNewYear()
        {
            /*if (!_unlockedButtons.Contains(_mainPanel.NewYearButton))
            {
                _unlockedButtons.Add(_mainPanel.NewYearButton);
            }*/
        }

        ~MainButtonsController()
        {
            GlobalEvent.LoadGameEvent -= LoadTutorialButtonsState;
            _tutorialStateStorage.StartWatchTutorialEvent -= AddTutorialButton;
            _newYearService.NewYearProjectsActivatedEvent -= OnNewYear;
        }
        
        
        
        public void Initialize() {}

        public void CaptureControl(MainUIButton button, bool isShow)
        {
            if (_unlockedButtons.Contains(button))
            {
                _unlockedButtons.Remove(button);
            }

            if (isShow)
            {
                button.Show();
            }
            else
            {
                button.HideNoAnimation();
            }
            
        }

        public void FreeControl(MainUIButton button)
        {
            if (!_unlockedButtons.Contains(button))
            {
                _unlockedButtons.Add(button);
            }
        }

        public void Tick()
        {
            if (UIWindowObserver.CurrentOpenedPanel is null && !IsWatchingCinema)
            {
                if (_showButtonsTimer > 0)
                {
                    _showButtonsTimer -= Time.deltaTime;
                    
                    if (_showButtonsTimer <= 0)
                    {
                        ShowButtons();
                    }
                }
            }
            else
            {
                _showButtonsTimer = ShowButtonsDelay;
                HideButtons();
            }
        }

        private void ShowButtons()
        {
            foreach (var button in _unlockedButtons)
            {
                button.Show();
            }
        }
        private void HideButtons(bool animated = false)
        {
            foreach (var button in _unlockedButtons)
            {
                if (animated)
                {
                    button.Hide();
                }
                else
                {
                    button.HideNoAnimation();
                }
            }
        }
        private void LoadTutorialButtonsState()
        {
            for (int i = 0; i < 5; i++)
            {
                var isWatched = _tutorialStateStorage.TutorialData[i].IsWatched;
                var buttons = _mainPanel.GetPanelButtonsByTutorialId(i);
                if (buttons == null)
                {
                    continue;
                }

                foreach (var button in buttons)
                {
                    if (isWatched || !_cutSceneConfig.IsShowCutscene || _cutsceneStartController.GetLastWatchedSceneId() >= 4)
                    {
                        _unlockedButtons.Add(button);
                    }
                    else
                    {
                        button.DisableButton();
                    }
                }
            }
        }


        private void AddTutorialButton(int id)
        {
            var buttons = _mainPanel.GetPanelButtonsByTutorialId(id);
            if (buttons == null)
            {
                return;
            }
            
            foreach (var button in buttons)
            {
                if (_unlockedButtons.Contains(button)) continue;
                
                button.EnableButton();
                _unlockedButtons.Add(button);

            }
        }
        
        private bool IsWatchingCinema
            => CutSceneActionsExecutor.IsWatching || CloudCurtain.IsAnimating;
        
    }
}