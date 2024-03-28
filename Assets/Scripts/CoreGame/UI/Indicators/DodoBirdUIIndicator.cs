using DG.Tweening;
using System;
using UnityEngine;
using VillageGame.UI.Indicators;

namespace CoreGame.UI.Indicators
{
    public class DodoBirdUIIndicator : UIIndicator
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _bird;
        [SerializeField] private GameObject _value;

        public void LaunchDodoBird(Vector3 startPosition, Action callback)
        {
            _bird.SetActive(true);
            _bird.transform.position = startPosition;
            _bird.transform.localScale = Vector3.one;

            _bird.transform.DOMove(_target.position, 1f).SetEase(Ease.OutQuint);
            _bird.transform.DOScale(0.25f, 1f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                callback?.Invoke();
                _bird.SetActive(false);
            });
        }

        public void BounceValue()
        {
            _value.transform.DOScale(2f, 0.25f);
        }
    }
}
