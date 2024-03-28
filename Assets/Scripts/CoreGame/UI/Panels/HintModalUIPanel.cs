using DG.Tweening;
using SO;
using UnityEngine;
using Util;
using VillageGame.Services;
using VillageGame.UI.Buttons;
using VillageGame.UI.Indicators;

namespace CoreGame.UI.Panels
{
    public class HintModalUIPanel : ModalUIPanel
    {
        [SerializeField] private RemoveDodoCoinButton _buyHintButton;
        [SerializeField] private DodoCoinsUIIndicator _dodoCoinsCounter;

        protected override void OnAwake()
        {
            base.OnAwake();
            _dodoCoinsCounter.UpdateValue(DodoCoinService.CurrentBalance.ToString());
        }

        protected override void OnShown()
        {
            _buyHintButton.SetData(5,$"{Extensions.GenerateRandomIdempotencyKey()}","hint",BuyHint);
        }

        private void BuyHint()
        {
            StaticPrefs.CoreGameHints++;
            var s = DOTween.Sequence();
            s.AppendInterval(0.5f);
            s.AppendCallback(Hide);
        }
        
    }
}