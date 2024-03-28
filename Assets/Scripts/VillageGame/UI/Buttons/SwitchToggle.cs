using UnityEngine;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class SwitchToggle : MonoBehaviour
    {
        [SerializeField] private GameObject _body;
        [SerializeField] private GameObject _onButton;
        [SerializeField] private GameObject _offButton;

        [SerializeField] private NewYearProjectSubPanel _panel;

        private bool _isOn = true;

        public void OnBackgroundClick()
        {
            _isOn = !_isOn;
            UpdateState(_isOn);
            if(_isOn)
            {
                _panel.ActivateProject();
            }
            else
            {
                _panel.DeactivateProject();
            }
        }

        public void OnOnClick()
        {
            _isOn = false;
            UpdateState(_isOn);
            _panel.DeactivateProject();
        }

        public void OnOffClick()
        {
            _isOn = true;
            UpdateState(_isOn);
            _panel.ActivateProject();
        }

        public void SetState(bool isOn)
        {
            _isOn = isOn;
            UpdateState(_isOn);
        }

        public void Show()
        {
            _body.SetActive(true);
        }

        public void Hide()
        {
            _body.SetActive(false);
        }

        private void UpdateState(bool isOn)
        {
            _onButton.SetActive(_isOn);
            _offButton.SetActive(!_isOn);
        }
    }
}
