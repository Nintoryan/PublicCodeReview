using DG.Tweening;
using UnityEngine;
using VillageGame.Services;
using VillageGame.UI.Buttons;

namespace VillageGame.UI.Panels.Warnings
{
    public class YearPizzaInfoWarningPanel : WarningPanel
    {
        [SerializeField] private EventButton _okButton;
        [SerializeField] private CalendarPanel _calendarPanel;
        
        
        protected override void SubscribeToEvents(bool flag)
        {
            base.SubscribeToEvents(flag);
            if (flag)
            {
                _okButton.ClickEvent += OnOkClick;
            }
            else
            {
                _okButton.ClickEvent -= OnOkClick;
            }
        }
        private void OnOkClick()
        {
            DodoBirdsService.IsYearPizzaInfoWatched = true;
            WherePresentsService.Watch_YearPizzaInfo();
            var s = DOTween.Sequence();
            s.AppendCallback(()=>Hide());
            s.AppendInterval(0.71f);
            s.AppendCallback(() => _calendarPanel.Show());
        }
    }
}