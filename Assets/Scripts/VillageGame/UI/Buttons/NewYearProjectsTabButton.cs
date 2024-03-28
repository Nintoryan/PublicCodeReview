using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class NewYearProjectsTabButton : UIButton
    {
        [SerializeField] private NewYearProjectsPanel _newYearProjectsPanel;
        [SerializeField] private bool _isCompleted;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _name;

        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _inactiveColor = Color.gray;
        [SerializeField] private Color _activeTextColor = Color.white;
        [SerializeField] private Color _inactiveTextColor = Color.gray;

        public void UpdateTab(bool isCompleted)
        {
            _image.color = isCompleted == _isCompleted ? _activeColor : _inactiveColor;
            _name.color = isCompleted == _isCompleted ? _activeTextColor : _inactiveTextColor;
        }

        protected override void OnClick()
        {
            _newYearProjectsPanel.OnTabButtonClick(_isCompleted);
        }
    }
}
