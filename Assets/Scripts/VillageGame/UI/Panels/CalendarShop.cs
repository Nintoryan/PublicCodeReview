using System;
using TMPro;
using UnityEngine;

namespace VillageGame.UI.Panels
{
    public class CalendarShop : MonoBehaviour
    {
        [Header("Gift shop")]
        [Space]
        [Header("Bought")]
        [SerializeField] private GameObject _boughtSelected;
        [SerializeField] private GameObject _boughtDeselected;
        [SerializeField] private GameObject _boughtScroll;
        public Transform _boughtContent;

        [Header("Shop")]
        [SerializeField] private GameObject _shopSelected;
        [SerializeField] private GameObject _shopDeselected;
        [SerializeField] private GameObject _shopScroll;
        [SerializeField] private TMP_Text _timeToRefresh;
        
        public Transform _shopContent;

        private void FixedUpdate()
        {
            _timeToRefresh.text = TimeUntilMidnight();
        }
        
        static string TimeUntilMidnight()
        {
            var now = DateTime.Now;
            var midnight = now.Date.AddDays(1);
            var timeRemaining = midnight - now;

            var formattedTime = $"{timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
            return formattedTime;
        }
        

        public void SelectShop()
        {
            _boughtDeselected.SetActive(true);
            _boughtSelected.SetActive(false);
            _shopSelected.SetActive(true);
            _shopDeselected.gameObject.SetActive(false);
            
            _shopScroll.gameObject.SetActive(true);
            _boughtScroll.gameObject.SetActive(false);
        }
        
        public void SelectBought()
        {
            _boughtDeselected.SetActive(false);
            _boughtSelected.SetActive(true);
            _shopSelected.SetActive(false);
            _shopDeselected.gameObject.SetActive(true);
            
            _shopScroll.gameObject.SetActive(false);
            _boughtScroll.gameObject.SetActive(true);
        }
    }
}