using System;
using DG.Tweening;
using SO;
using TMPro;
using UnityEngine;
using VContainer;
using VillageGame.Services.Storages;
using VillageGame.UI.Indicators;
using VillageGame.UI.Panels;
using VillageGame.UI.Panels.Warnings;
using EnergyUIIndicator = CoreGame.UI.Indicators.EnergyUIIndicator;

namespace CoreGame.UI.Panels
{
    public class EndGameUIPanel : UIPanel
    {
        [SerializeField] protected SceneLoader _sceneLoader;
        
        [SerializeField] protected LevelUIIndicator _levelUIIndicator;
        [SerializeField] protected CurrencyUIIndicator _currencyUIIndicator;
        [SerializeField] protected EnergyUIIndicator _energyUIIndicator;
        [SerializeField] protected MainUIPanel _mainUIPanel;
        protected float _offset = 500f;
        
        private CurrencyStorage _currencyStorage;
        private RatingStorage _ratingStorage;
        private EnergyStorage _energyStorage;
        private EnergyWarningUIPanel _warningUIPanel;

        [Inject]
        public void Initialize(IObjectResolver resolver)
        {
            try
            {
                _currencyStorage = resolver.Resolve<CurrencyStorage>();
                _ratingStorage = resolver.Resolve<RatingStorage>();
                _energyStorage = resolver.Resolve<EnergyStorage>();
                _warningUIPanel = resolver.Resolve<EnergyWarningUIPanel>();
            }
            catch (Exception)
            {
                // ignored
            }

            _levelUIIndicator.transform.localPosition += Vector3.up * _offset;
            _currencyUIIndicator.transform.localPosition += Vector3.up * _offset;
            _energyUIIndicator.transform.localPosition += Vector3.up * _offset;
        }

        public override void Show(Action onShown = null)
        {
            base.Show(onShown);
            _energyUIIndicator.UpdateValue(StaticPrefs.EnergyValue.ToString());
            _mainUIPanel.HideEndgameTimer();
            OnShown();
        }

        public void ShowIndicators()
        {
            _levelUIIndicator.transform.DOLocalMoveY(_levelUIIndicator.transform.localPosition.y - _offset, 1f);

            _currencyUIIndicator.transform.DOLocalMoveY(_currencyUIIndicator.transform.localPosition.y - _offset, 1f);

            _energyUIIndicator.transform.DOLocalMoveY(_energyUIIndicator.transform.localPosition.y - _offset, 1f);
        }

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }
        

        protected virtual void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                if (_currencyStorage != null)
                {
                    _currencyStorage.Change += OnUpdateCurrencyValue;
                    OnUpdateCurrencyValue(_currencyStorage.Current);
                }

                if (_ratingStorage != null)
                {
                    _ratingStorage.ChangeLevel += OnUpdateLevelValue;
                    _ratingStorage.ChangeRaiting += OnUpdateRatingValue;
                    OnUpdateLevelValue(_ratingStorage.Level);
                    OnUpdateRatingValue(_ratingStorage.Raiting, _ratingStorage.RatingRequirement);
                }
                StaticPrefs.EnergyUpdatedEvent += OnEnergyUpdateEvent;
            }
            else
            {
                if (_currencyStorage != null)
                {
                    _currencyStorage.Change -= OnUpdateCurrencyValue;
                }

                if (_ratingStorage != null)
                {
                    _ratingStorage.ChangeLevel -= OnUpdateLevelValue;
                    _ratingStorage.ChangeRaiting -= OnUpdateRatingValue;
                }
                
                StaticPrefs.EnergyUpdatedEvent -= OnEnergyUpdateEvent;
            }
        }

        public void ReloadScene(bool isVictory)
        {
            if (_energyStorage.IsEnoughEnergy(1))
            {
                _sceneLoader.ReloadScene(isVictory);
            }
            else
            {
                _warningUIPanel.Show();
            }
        }


        private void OnUpdateLevelValue(int value) => _levelUIIndicator.UpdateValue(value.ToString());
        private void OnUpdateCurrencyValue(int value) => _currencyUIIndicator.UpdateValue(value.ToString());
        private void OnUpdateRatingValue(int current, int max) => _levelUIIndicator.UpdateExp(current,max);
        protected virtual void OnEnergyUpdateEvent() => _energyUIIndicator.UpdateValue(StaticPrefs.EnergyValue.ToString());
        public void UnloadScene(bool isVictory) => _sceneLoader.UnloadScene(isVictory);
    }
}