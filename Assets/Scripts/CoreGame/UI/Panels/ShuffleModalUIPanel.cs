using DG.Tweening;
using SO;
using UnityEngine;
using Util;
using VillageGame.Services;
using VillageGame.UI.Buttons;
using VillageGame.UI.Indicators;

namespace CoreGame.UI.Panels
{
    public class ShuffleModalUIPanel : ModalUIPanel
    {
        [SerializeField] private RemoveDodoCoinButton _buyShuffleButton;
        [SerializeField] private DodoCoinsUIIndicator _dodoCoinsCounter;

        protected override void OnAwake()
        {
            base.OnAwake();
            _dodoCoinsCounter.UpdateValue(DodoCoinService.CurrentBalance.ToString());
        }

        protected override void OnShown()
        {
            _buyShuffleButton.SetData(2,$"{Extensions.GenerateRandomIdempotencyKey()}","shuffle",BuyHint);
        }

        private void BuyHint()
        {
            StaticPrefs.CoreGameShuffles++;
            var s = DOTween.Sequence();
            s.AppendInterval(0.5f);
            s.AppendCallback(Hide);
        }
    }
}