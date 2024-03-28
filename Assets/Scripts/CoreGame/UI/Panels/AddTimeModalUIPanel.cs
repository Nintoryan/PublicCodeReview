using DG.Tweening;
using UnityEngine;
using Util;
using VillageGame.Services;
using VillageGame.UI.Buttons;
using VillageGame.UI.Indicators;

namespace CoreGame.UI.Panels
{
    public class AddTimeModalUIPanel : ModalUIPanel
    {
        [SerializeField] private LevelTimer _levelTimer;
        [SerializeField] private RemoveDodoCoinButton _addTimeButton;
        [SerializeField] private DefeatUIPanel _defeatUIPanel;
        [SerializeField] private DodoCoinsUIIndicator _dodoCoinsCounter;

        protected override void OnAwake()
        {
            _close.onClick.AddListener(()=>
            {
                Hide(()=>_defeatUIPanel.Show());
            });
            _dodoCoinsCounter.UpdateValue(DodoCoinService.CurrentBalance.ToString());
        }
        
        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        protected override void OnShown()
        {
            _addTimeButton.SetData(5,$"{Extensions.GenerateRandomIdempotencyKey()}","time",AddBonusTime);
            _dodoCoinsCounter.UpdateValue(DodoCoinService.CurrentBalance.ToString());
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _levelTimer.TimerEndedEvent += OnTimerEndedEvent;
            }
            else
            {
                _levelTimer.TimerEndedEvent -= OnTimerEndedEvent;
            }
        }

        private void OnTimerEndedEvent()
        {
            Show();
        }

        private void AddBonusTime()
        {
            _levelTimer.AddTime(30);
            
            var s = DOTween.Sequence();
            s.AppendInterval(0.5f);
            s.AppendCallback(Hide);
        }
    }
}