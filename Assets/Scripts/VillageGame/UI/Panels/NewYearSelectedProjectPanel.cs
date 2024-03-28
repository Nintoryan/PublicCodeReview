using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels
{
    public class NewYearSelectedProjectPanel : UIPanel
    {
        [SerializeField] private NewYearProjectPurchaseButton _newYearProjectPurchaseButton;
        [SerializeField] private Image _projectImage;
        [SerializeField] private TextMeshProUGUI _projectDescriptionText;
        [SerializeField] private TextMeshProUGUI _projectBonusValueText;
        [SerializeField] private TMP_Text _header;
        

        public void Initialize(Sprite projectImage, string header, string description, string bonusValue)
        {
            _projectImage.sprite = projectImage;
            _projectDescriptionText.text = description;
            _projectBonusValueText.text = bonusValue;
            _header.text = header;
            
            _projectImage.rectTransform.sizeDelta = projectImage.rect.size * 1.8f;
        }

        public void Close()
        {
            transform.parent.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack);
        }
    }
}
