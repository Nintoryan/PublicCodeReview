using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Controllers
{
    public class BuildingCanvasController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject _canvas;
        [SerializeField] private BuildingCanvasButton _buildButton;
        [SerializeField] private BuildingCanvasButton _deleteButton;
        private InputBlockService _inputBlockService;

        public event Action OnPressBuild;
        public event Action OnPressRemove;

        private void OnEnable()
        {
            _buildButton.SetHidden();
            _deleteButton.SetHidden();
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void OnDestroy()
        {
            if(_inputBlockService != null)
                _inputBlockService.PointerExit();
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _buildButton.ClickBuildEvent += PressBuild;
                _deleteButton.ClickBuildEvent += PressDelete;
            }
            else
            {
                _buildButton.ClickBuildEvent -= PressBuild;
                _deleteButton.ClickBuildEvent -= PressDelete;
            }
        }

        private void PressBuild() => OnPressBuild?.Invoke();
        private void PressDelete() => OnPressRemove?.Invoke();


        public void ShowDeleteButton(Action OnPress)
        {
            _canvas.SetActive(true);
            _deleteButton.Show();
            OnPressRemove = OnPress;
        }

        public void HideDeleteButton()
        {
            _deleteButton.Hide(onHidden: () => _canvas.SetActive(false));
            OnPressRemove = null;
        }

        public void ShowBuildButton(Action OnPress)
        {
            _canvas.SetActive(true);
            _buildButton.Show();
            OnPressBuild = OnPress;
        }

        public void HideBuildButton()
        {
            _buildButton.Hide(onHidden: () => _canvas.SetActive(false));
            OnPressBuild = null;
        }

        public void HideAllButtons()
        {
            _buildButton.Hide();
            _deleteButton.Hide(onHidden: () => _canvas.SetActive(false));
        }

        public void SetBlocker(InputBlockService inputBlockService)
        {
            _inputBlockService = inputBlockService;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _inputBlockService.PointerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inputBlockService.OnPointerExit(eventData);
        }
    }
}