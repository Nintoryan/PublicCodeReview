using System;
using System.Linq;
using SO;
using SO.Data.Presents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using VillageGame.Services;
using VillageGame.UI.Buttons;
using Web.Api;
using Web.ResponseStructs;
using Web.ResponseStructs.PayloadValues;

namespace VillageGame.UI.Panels
{
    public class PresentInformationPanel : UIPanel
    {
        [SerializeField] private TMP_Text _letterGreetings;
        [SerializeField] private TMP_Text _letterText;
        [SerializeField] private TMP_Text _letterSign;
        [SerializeField] private EventButton _closeButton;
        
        [Header("promocode")]
        [SerializeField] private GameObject _promocodeTicket;
        [SerializeField] private TMP_Text _promocodeDescription;
        [SerializeField] private TMP_Text _promocode;
        [SerializeField] private EventButton _copyButton;
        [SerializeField] private TMP_Text _copyButtonText;

        [Header("dodocoin")]
        [SerializeField] private GameObject _dodoCoinTicket;
        [SerializeField] private TMP_Text _dodoCoins;
        [SerializeField] private AddDodoCoinButton _getCoinsButton;
        [SerializeField] private TMP_Text _getCoinsDoneText;

        [Header("building")]
        [SerializeField] private GameObject _buildingTicket;
        [SerializeField] private Image _iconImage;
        [SerializeField] private AddBuildingButton _getBuildingButton;
        [SerializeField] private TMP_Text _getBuildingDoneText;
        
        [Header("я хочу умереть")] 
        [SerializeField] private GameObject _oldShit;
        [SerializeField] private GameObject _newShit;
        [SerializeField] private TMP_Text _giftDescription;
        [SerializeField] private TMP_Text _giftPromocode;
        [SerializeField] private EventButton _giftCopyButton;
        [SerializeField] private TMP_Text _gitfCopyButtonText;

        [Header("logo")] 
        [SerializeField] private GameObject _logoTicket;
        [SerializeField] private Image _logoIcon;
        
        public Action<PresentValueData> ClaimPresent;
        public PresentValueData PresentValueData { get; private set; }

        private bool _isOpened;
        
        private void OnEnable()
        {
            if(_copyButton == null || _getCoinsButton == null) return;
            
            _closeButton.ClickEvent += HideNoAction;
            JSAPI.Copied += OnCopied;
        }
        
        private void OnDisable()
        {
            if(_copyButton == null || _getCoinsButton == null) return;
            
            _closeButton.ClickEvent -= HideNoAction;
            JSAPI.Copied -= OnCopied;
        }
        
        private void OnCopied()
        {
            try
            {
                _copyButton.PlayImpact();
                _copyButtonText.text = "Скопирован";
                
                _giftCopyButton.PlayImpact();
                _gitfCopyButtonText.text = "Скопирован";
            }
            catch
            {
                // ignored
            }
        }


        public void SetPresentData(PresentValueData valueData, OpenedPresentBoxData data,bool isOpened = false)
        {
            if (valueData == null)
            {
                Debugging.Log($"Take present panel: cannot set present value, please check configs", LogStyle.Error);
                return;
            }
            
            _oldShit.gameObject.SetActive(true);
            _newShit.gameObject.SetActive(false);
            
            _copyButtonText.text = "Копировать";

            _isOpened = isOpened;
            PresentValueData = valueData;

            _letterGreetings.text = valueData.Letter_Greetings;
            _letterText.text = valueData.Letter_Text;
            _letterSign.text = valueData.Letter_Sign;
            
            _promocodeTicket.gameObject.SetActive(false);
            _dodoCoinTicket.gameObject.SetActive(false);
            _buildingTicket.gameObject.SetActive(false);
            _logoTicket.gameObject.SetActive(false);

            if (valueData is PromocodePresentValueData p)
            {
                _promocodeTicket.gameObject.SetActive(true);
                _promocodeDescription.text = p.Description;
                if (data != null)
                {
                    _promocode.text = data.StringValue;
                    JSAPI.ShowCopyToClipboard(data.StringValue);
                }
            }
            else if (valueData is DodoCoinsPresentValueData d)
            {
                if (isOpened)
                {
                    _getCoinsButton.gameObject.SetActive(false);
                    _getCoinsDoneText.gameObject.SetActive(true);
                }
                else
                {
                    _getCoinsButton.gameObject.SetActive(true);

                    _getCoinsButton.SetData(d.DodoCoinsValue,Extensions.GenerateRandomIdempotencyKey(),
                        d.DodoCoinsValue.ToString(),
                        GetDodoCoins);

                    _getCoinsDoneText.gameObject.SetActive(false);
                }

                _dodoCoinTicket.gameObject.SetActive(true);
                switch (d.DodoCoinsValue)
                {
                    case AddCoinsType.none:
                        _dodoCoins.text = "0";
                        break;
                    case AddCoinsType.coins_50:
                        _dodoCoins.text = "50";
                        break;
                    case AddCoinsType.coins_100:
                        _dodoCoins.text = "100";
                        break;
                    case AddCoinsType.coins_200:
                        _dodoCoins.text = "200";
                        break;
                }
            }
            else if (valueData is BuildPresentValueData b)
            {
                _buildingTicket.gameObject.SetActive(true);
                _iconImage.sprite = b.Icon;

                if (isOpened)
                {
                    _getBuildingButton.gameObject.SetActive(false);
                    _getBuildingDoneText.gameObject.SetActive(true);
                }
                else
                {
                    _getBuildingButton.gameObject.SetActive(true);
                    _getBuildingButton.SetData(GetBuilding);
                    _getBuildingDoneText.gameObject.SetActive(false);
                }
            }
            else
            {
                Debugging.Log("ПОДАРКИ МОГУТ БЫТЬ ТОЛЬКО ПРОМИКИ, ДОДОКОИНЫ И ЗДАНИЯ!!!", ColorType.Red);
            }
        }

        public override void Hide(Action onHidden = null)
        {
            base.Hide(onHidden);
            JSAPI.HideCopyToClipboard();
        }

        private void GetDodoCoins()
        {
            if (!_isOpened)
            {
                ClaimPresent?.Invoke(PresentValueData);
                _getCoinsButton.gameObject.SetActive(false);
                _getCoinsDoneText.gameObject.SetActive(true);
            }
        }

        private void GetBuilding()
        {
            if (!_isOpened)
            {
                _getBuildingButton.gameObject?.SetActive(false);
                _getBuildingDoneText.gameObject.SetActive(true);
                Invoke(nameof(InvokeHide), 1f);
            }
        }

        private void InvokeHide() => Hide();

  
        public void SetGiftData(DodoBirdGift gift)
        {
            var dodoBirdsGiftData = DodoBirdsService.Gifts.FirstOrDefault(g => g.id == gift.Id);

            if (dodoBirdsGiftData == null)
            {
                Debugging.Log("Set Gift Data Error",ColorType.Red);
                return;
            }
            
            _oldShit.gameObject.SetActive(false);
            _newShit.gameObject.SetActive(true);

            _giftPromocode.text = dodoBirdsGiftData.promocode;
            _giftDescription.text = gift.Description;
            
            SetLogoImage(gift.PartnerLogo);
            
            JSAPI.ShowCopyToClipboard(dodoBirdsGiftData.promocode);
        }
        
        private void SetLogoImage(Sprite logo)
        {
            if (logo != null)
            {
                _logoTicket.SetActive(true);
                _logoIcon.sprite = logo;
            }
            else
            {
                _logoTicket.SetActive(false);
            }
        }
    }
}