using System.Collections;
using DG.Tweening;
using UnityEngine;
using VillageGame.Logic.Curtains;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using VillageGame.UI.Buttons;
using VillageGame.UI.Services;

namespace VillageGame.UI.Panels.Warnings
{
    public class WhereAreThePresentsWarningPanel : WarningPanel
    {
        [SerializeField] private EventButton _okButton;
        [SerializeField] private WhereAreThePresentsWarningPanel _next;

        public void TryShowShit()
        {
            StartCoroutine(Zaloopa());
        }

        private IEnumerator Zaloopa()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(Free);
            Show();
        }

        private bool Free()
        {
            return UIWindowObserver.CurrentOpenedPanel is null &&
                   !(CutSceneActionsExecutor.IsWatching || CloudCurtain.IsAnimating);
        }

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
            if (_next == null)
            {
                WherePresentsService.Watch_WherePresents();
                Hide();
            }
            else
            {
                var s = DOTween.Sequence();
                s.AppendCallback(()=>Hide());
                s.AppendInterval(0.71f);
                s.AppendCallback(() => _next.Show());
            }
        }
    }
}