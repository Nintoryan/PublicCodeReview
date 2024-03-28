using System.Collections;
using System.Collections.Generic;
using SO.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;
using VContainer;
using VillageGame.Logic.Curtains;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using VillageGame.Services.CutScenes.CustomActions;
using VillageGame.UI.Panels;

namespace VillageGame.UI
{
    public class InputBlockService : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private InputService _inputService;
        private UIFacade _uiFacade;
        private bool _isOpenWindow, _onStayUI;

        private Coroutine _coroutineStayUI, _coroutineOpenWindow;
        private int _openedPanelsCount;

        private bool _isDestoyed;
        private bool _isSubscribe;

        private readonly List<UIPanel> _openedPanels = new();

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _inputService = objectResolver.Resolve<InputService>();
            _uiFacade = objectResolver.Resolve<UIFacade>();
            SubscribeToEvent(true);
        }

        private void OnDestroy()
        {
            _isDestoyed = true;
            if (_coroutineStayUI != null)
            {
                StopCoroutine(_coroutineStayUI);
            }

            SubscribeToEvent(false);
        }

        private void SubscribeToEvent(bool flag)
        {
            if (_isSubscribe == flag)
            {
                return;
            }

            _isSubscribe = flag;
            if (flag)
            {
                CutSceneActionsExecutor.EndEvent += SetInputBlock;
                CutSceneStateObserver.EndCutsceneEvent += OnEndCutscene;
                CloudCurtain.EndAnimationEvent += SetInputBlock; 
                foreach (var panel in _uiFacade.Panels)
                {
                    if (!panel.IsBlockInput) continue;
                    panel.PanelSwitchEvent += OnPanelSwitchEvent;
                }
            }
            else
            {
                CutSceneActionsExecutor.EndEvent -= SetInputBlock;
                CutSceneStateObserver.EndCutsceneEvent -= OnEndCutscene;
                CloudCurtain.EndAnimationEvent -= SetInputBlock; 
                foreach (var panel in _uiFacade.Panels)
                {
                    if (!panel.IsBlockInput) continue;
                    panel.PanelSwitchEvent -= OnPanelSwitchEvent;
                }
            }
        }

        private void OnEndCutscene(CutSceneData obj) => SetInputBlock();


        public void OnPointerEnter(PointerEventData eventData) => PointerEnter();

        public void OnPointerExit(PointerEventData eventData) => PointerExit();

        public void PointerEnter()
        {
            _onStayUI = true;
            SetInputBlock();
        }

        public void PointerExit()
        {
            if (_isDestoyed || !gameObject.activeSelf) return;
            _coroutineStayUI = StartCoroutine(PointerExitWithDelay());
        }

        private IEnumerator PointerExitWithDelay()
        {
            yield return new WaitForSecondsRealtime(0.15f);
            if (_isDestoyed)
            {
                yield break;
            }

            _onStayUI = false;
            SetInputBlock();
        }

        private void OnPanelSwitchEvent(UIPanel panel, bool isOpen)
        {
            if ((isOpen && _openedPanels.Contains(panel)) || !isOpen && !_openedPanels.Contains(panel))
            {
                return;
            }

            if (isOpen) _openedPanels.Add(panel);
            else _openedPanels.Remove(panel);
            RefreshOpenedPanelCount(isOpen);
            _isOpenWindow = _openedPanelsCount > 0;
            SetInputBlock();
        }

        private void RefreshOpenedPanelCount(bool isOpen)
        {
            if (isOpen)
            {
                _openedPanelsCount++;
            }
            else
            {
                _openedPanelsCount--;
            }
        }

        private void SetInputBlock()
        {
            if (Constance.IsWatchingCinema())
            {
                if (!_inputService.IsBlock)
                {
                    _inputService.BlockInput(true);
                }

                return;
            }

            _inputService.BlockInput(_isOpenWindow || _onStayUI);
        }
    }
}