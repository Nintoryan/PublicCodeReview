using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Services;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class NewYearProjectSubPanel : UIPanel
    {
        [SerializeField] private MPImage _backGround;
        [SerializeField] private TextMeshProUGUI _projectNameText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private SwitchToggle _switchToggle;
        [SerializeField] private NewYearProjectSelectButton _selectButton;
        [SerializeField] private GameObject _lockPanel;

        [SerializeField] private NewYearProjectsPanel _newYearProjectsPanel;

        [Header("Active")]
        [SerializeField] private TMP_Text _activeHeader;

        [SerializeField] private float _activeHeight;
        
        [Header("Unactive")] 
        [SerializeField] private float _unactiveHeight;

        private RectTransform _rectTransform;
        

        private ProjectState _projectState;
        private int _id;

        public void Initialize(NewYearProjectPresentation newYearProjectPresentation, NewYearProjectsPanel newYearProjectsPanel)
        {
            _newYearProjectsPanel = newYearProjectsPanel;
            _rectTransform = GetComponent<RectTransform>();
            _projectNameText.text = newYearProjectPresentation.ProjectName;
            _activeHeader.text = newYearProjectPresentation.ProjectName;
            _id = newYearProjectPresentation.ID;
            _iconImage.sprite = newYearProjectPresentation.Icon;
            
            if (newYearProjectPresentation.Icon != null)
            {
                _iconImage.rectTransform.sizeDelta = newYearProjectPresentation.Icon.rect.size * 0.9f;
            }
            
            _projectState = newYearProjectPresentation.State;

            switch (_projectState)
            {
                case ProjectState.Locked:
                    SetOutLine(false);
                    _projectNameText.gameObject.SetActive(true);
                    _activeHeader.gameObject.SetActive(false);
                    _iconImage.enabled = true;
                    
                    _switchToggle.Hide();
                    _selectButton.Hide();
                    _lockPanel.SetActive(true);

                    _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _unactiveHeight);
                    
                    break;
                case ProjectState.Unlocked:
                    SetOutLine(false);
                    _projectNameText.gameObject.SetActive(true);
                    _activeHeader.gameObject.SetActive(false);
                    _iconImage.enabled = true;
                    
                    _switchToggle.Hide();
                    _selectButton.Show();
                    _lockPanel.SetActive(false);
                    
                    _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _unactiveHeight);
                    break;
                case ProjectState.Active:
                    SetOutLine(true);
                    _projectNameText.gameObject.SetActive(false);
                    _activeHeader.gameObject.SetActive(true);
                    _activeHeader.color = DodoColors.TextOrange;
                    _iconImage.enabled = false;
                    
                    _switchToggle.Show();
                    _selectButton.Hide();
                    _lockPanel.SetActive(false);
                    _switchToggle.SetState(true);
                    
                    _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _activeHeight);
                    break;
                case ProjectState.Inactive:
                    SetOutLine(false);
                    _projectNameText.gameObject.SetActive(false);
                    _activeHeader.gameObject.SetActive(true);
                    _activeHeader.color = DodoColors.TextGray;
                    _iconImage.enabled = false;
                    

                    _switchToggle.Show();
                    _selectButton.Hide();
                    _lockPanel.SetActive(false);
                    _switchToggle.SetState(false);
                    
                    _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _activeHeight);
                    break;
            }
        }

        public void SetOutLine(bool isBold)
        {
            _backGround.OutlineWidth = isBold ? 12f : 2f;
            _backGround.OutlineColor = isBold ? DodoColors.TextOrange : DodoColors.LightBrown;
        }

        public void SetInsufficientCurrency()
        {
            _selectButton.Deactivate();
        }

        public void OnLaunchButtonClick()
        {
            _newYearProjectsPanel.LaunchProject(_id);
        }

        public void ActivateProject()
        {
            SetOutLine(true);
            _activeHeader.color = DodoColors.TextOrange;
            _newYearProjectsPanel.ActivateProject(_id);
        }

        public void DeactivateProject()
        {
            SetOutLine(false);
            _activeHeader.color = DodoColors.TextGray;
            _newYearProjectsPanel.DeactivateProject(_id);
        }
    }

    public enum ProjectState
    {
        None,
        Locked,
        Unlocked,
        Active,
        Inactive
    }
}
