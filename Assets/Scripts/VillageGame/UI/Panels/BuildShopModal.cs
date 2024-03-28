using Data.Scripts.Audio;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class BuildShopModal : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private SkeletonGraphic _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _forBuild;
        [SerializeField] private TMP_Text _forHour;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private Button _buildButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _backButtonObject;
        [SerializeField] private GameObject _purchasedButtonContent;
        public Button BuildButton => _buildButton;

        public UnityAction<ShopBuildingUIButton.Data> BuildButtonClickedEvent;
        public UnityAction CloseButtonClickedEvent;

        private ShopBuildingUIButton.Data _currentData;

        private void Awake()
        {
            _buildButton.onClick.AddListener(() => BuildButtonClickedEvent?.Invoke(_currentData));
            _closeButton.onClick.AddListener(() => CloseButtonClickedEvent?.Invoke());
        }

        public void DisableBackButton()
        {
            _backButtonObject.SetActive(false);
        }

        public void EnableBackButton()
        {
            _backButtonObject.SetActive(true);
        }
        
        public void Load(ShopBuildingUIButton.Data data)
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.MenuOpen);
            _name.SetText(data.PresentationModel.Name);
            _description.SetText(data.PresentationModel.Description);
            _forBuild.SetText($"+{data.PresentationModel.RatingAfterConstruction}");
            _forHour.SetText($"+{data.PresentationModel.RatingPerHour}");
            if (data.ButtonMode == ShopBuildingUIButton.Mode.Purchased)
            {
                _purchasedButtonContent.SetActive(true);
            }
            else
            {
                _purchasedButtonContent.SetActive(false);
                _price.SetText($"{data.PresentationModel.Price}");
            }

            if (data.PresentationModel.BigIcon != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.skeletonDataAsset = data.PresentationModel.BigIcon;
                _icon.Initialize(true);
                _iconImage.gameObject.SetActive(false);
            }
            else
            {
                _iconImage.gameObject.SetActive(true);
                _iconImage.sprite = data.PresentationModel.Icon;
                _icon.gameObject.SetActive(false);
            }
            

            _currentData = data;
        }
    }
}