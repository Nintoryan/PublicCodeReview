using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Util;
using VillageGame.Logic.Curtains;
using VillageGame.Services.CutScenes;
using VillageGame.UI.Indicators;

namespace VillageGame.UI.Panels
{
    public class LoadingCurtainPanel : UIPanel
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _hideDuration = 1;
        [SerializeField] private TextMeshProUGUI _loadingText;

        [SerializeField] private CloudCurtain _cloudCurtain;

        private bool _isHiding;
        

        private void Awake()
        {
            Debugging.Log($"LoadingCurtain: awake", ColorType.Red);
            Show();
        }

        public override void Show(Action onShown = null)
        {
            _canvasGroup.alpha = 1;
            base.Show(onShown);
        }

        public override void Hide(Action onHidden = null)
        {
            if (_isHiding)
            {
                return;
            }

            _isHiding = true;
            _cloudCurtain.Show(onShown: () =>
            {
                _canvasGroup.alpha = 0;
                base.Hide(onHidden);
            }, onHidden: () => _isHiding = false);
        }

        public void SetTextAsync(string text)
        {
            _loadingText.SetText(text);
        }
    }
}