using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VillageGame.Data.Types;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Buttons
{
    public class BuildingsTabUIButton : UIButton
    {
        [SerializeField] private BuildShopUIPanel buildPanel;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _name;
        

        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _inactiveColor = Color.gray;
        [SerializeField] private Color _activeTextColor = Color.white;
        [SerializeField] private Color _inactiveTextColor = Color.gray;
        [SerializeField] private BuildingType _type;

        public void UpdateTab(BuildingType type)
        {
            _image.color = _type == type ? _activeColor : _inactiveColor;
            _name.color = _type == type ? _activeTextColor : _inactiveTextColor;
        }

        protected override void OnClick()
        {
            buildPanel.SetFolder(_type);
        }
    }
}
