using System;
using System.Collections.Generic;
using SO;
using UnityEngine;
using VContainer;
using VillageGame.Services.LoadingData;
using VillageGame.UI;
using VillageGame.UI.Panels.Warnings;
using Web.RequestStructs;

namespace VillageGame.Services
{
    public class DodoBirdsService : ILoading
    {
        public static event Action GiftsUpdated;
        public static event Action BalanceUpdated;

        private static DodoBirdsData _currentData = new();
        private static bool _requestDone = true;
        private static DodoBirdGift _currentGift;
        private static Action<string> _currentSuccess;
        private static Action _currentFail;
        
        public static int Balance => _currentData.balance;
        public static List<DodoBirdsGiftData> Gifts => _currentData.gifts;
        
        private static DodoBirdsGiftsConfig _config;

        private WhereAreThePresentsWarningPanel _whereAreThePresentsWarningPanel;

        public static bool IsYearPizzaInfoWatched;


        [Inject]
        public DodoBirdsService(IObjectResolver resolver)
        {
            _whereAreThePresentsWarningPanel =
                resolver.Resolve<UIFacade>().FindWarningPanel<WhereAreThePresentsWarningPanel>();
            
            _config = resolver.Resolve<DodoBirdsGiftsConfig>();
        }
        
        public static void BuyDodoBirdsGift(int id, Action<string> onSuccess, Action onFail,string idempotencyKey)
        {
            if(!_requestDone || idempotencyKey == "") return;
            
            _currentGift = _config.GetGiftByID(id);

            if (_currentGift == null)
            {
                Debug.LogError($"Gift with id:{id} not found!");
                onFail?.Invoke();
                return;
            }

            _currentFail = onFail;
            
            if (Balance >= _currentGift.Price)
            {
                _currentSuccess = onSuccess;
                _requestDone = false;
                PromoCodeService.GetPromoCode(id,idempotencyKey,BirdBuySuccess,BirdBuyFail);
            }
            else
            {
                _currentGift = null;
                _currentFail?.Invoke();
                _currentFail = null;
            }
        }

        private static void BirdBuyFail()
        {
            _requestDone = true;
            _currentFail?.Invoke();
            
            _currentSuccess = null;
            _currentFail = null;
        }

        private static void BirdBuySuccess(string promoCode)
        {
            _requestDone = true;
            _currentData.balance -= _currentGift.Price;
            _currentData.gifts.Add(new DodoBirdsGiftData
            {
                id = _currentGift.Id,
                promocode = promoCode
            });
            
            BalanceUpdated?.Invoke();
            GiftsUpdated?.Invoke();
            
            _currentSuccess?.Invoke(promoCode);
            _currentSuccess = null;
        }

        public void AddDodoBirdAmount(int amount)
        {
            _currentData.balance+=amount;
            BalanceUpdated?.Invoke();
        }

        public void AddDodoBird()
        {
            _currentData.balance++;
            BalanceUpdated?.Invoke();
        }

        public void Load(LoadData request)
        {
            _currentData = new DodoBirdsData
            {
                balance = request.data.dodo_birds_balance,
                gifts = request.data.received_birds_gifts
            };

            _currentData.gifts ??= new List<DodoBirdsGiftData>();
            BalanceUpdated?.Invoke();
            GiftsUpdated?.Invoke();

            if (request.data.player_progress.level > 2 && !request.data.where_presents_watched_2)
            {
                _whereAreThePresentsWarningPanel.TryShowShit();
            }

            IsYearPizzaInfoWatched = request.data.year_pizza_info_watched;

        }
    }
    
    [Serializable]
    public class DodoBirdsData
    {
        public int balance;
        public List<DodoBirdsGiftData> gifts = new();
    }

    [Serializable]
    public class DodoBirdsGiftData
    {
        public int id;
        public string promocode;
    }
}