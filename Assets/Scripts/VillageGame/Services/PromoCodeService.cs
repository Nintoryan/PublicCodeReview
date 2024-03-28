using System;

namespace VillageGame.Services
{
    public static class PromoCodeService
    {
        public static event Action<int, string> IssuePromoCode;

        public static event Action<int, string> PromoCodeSuccess;

        private static Action<string> _success;
        private static Action _error;
        private static int _promoID;

        public static void GetPromoCode(int promoID, string idempotencyKey, Action<string> success, Action error)
        {
            _promoID = promoID;
            _success = success;
            _error = error;
            
            IssuePromoCode?.Invoke(promoID,idempotencyKey);
        }
        
        public static void PromoCodeReceived(string promoCode)
        {
            _success?.Invoke(promoCode);
            PromoCodeSuccess?.Invoke(_promoID,promoCode);
            _success = null;
            _error = null;
            _promoID = -1;

            /*_currentData.balance -= _currentGift.Price;
            BalanceUpdated?.Invoke();
            
            _currentData.gifts.Add(new DodoBirdsGiftData()
            {
                id = _currentGift.Id,
                promocode = promocode
            });
            
            GiftsUpdated?.Invoke();*/



            /*_currentGift = null;*/

            /*_requestDone = true;*/
        }

        public static void PromoCodeError()
        {
            _error?.Invoke();
            _success = null;
            _error = null;
            _promoID = -1;
            
            /*_currentGift = null;
            _requestDone = true;*/
        }
    }
}