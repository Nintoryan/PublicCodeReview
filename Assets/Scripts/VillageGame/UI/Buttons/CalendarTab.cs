using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class CalendarTab : UIButton
    {
        [SerializeField] private CalendarPanel _calendarPanel;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _name;
        

        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _inactiveColor = Color.gray;
        [SerializeField] private Color _activeTextColor = Color.white;
        [SerializeField] private Color _inactiveTextColor = Color.gray;
        [SerializeField] private bool _isShop;

        public void UpdateTab(bool isShop)
        {
            _image.color = isShop == _isShop ? _activeColor : _inactiveColor;
            _name.color = isShop == _isShop ? _activeTextColor : _inactiveTextColor;
        }

        protected override void OnClick()
        {
            _calendarPanel.SetFolder(_isShop);
        }
    }
}