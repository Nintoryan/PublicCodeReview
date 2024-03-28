using System;
using VContainer;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;
using Web.ResponseStructs;

namespace VillageGame.Services
{
    public class DodoCoinService : ILoading
    {
        public static int CurrentBalance { get; private set; }
        
        public static event Action<AddCoinsType,string> SendCoins;
        public static event Action<int,string> RemoveCoins;

        private static Action _withdrawSuccess;
        private static Action _withdrawError;
        
        private static Action _addSuccess;
        private static Action _addError;

        private static int _spentAmount;
        private static int _addAmount;

        [Inject]
        public DodoCoinService()
        {
            
        }

        public static void AddCoins(AddCoinsType type, string key, Action onSuccess, Action onError)
        {
            switch (type)
            {
                case AddCoinsType.coins_50:
                    _addAmount = 50;
                    break;
                case AddCoinsType.coins_100:
                    _addAmount = 100;
                    break;
                case AddCoinsType.coins_200:
                    _addAmount = 200;
                    break;
            }
            SendCoins?.Invoke(type,key);

            _addSuccess = onSuccess;
            _addError = onError;
        }

        public static void AddCoinsSuccess()
        {
            CurrentBalance += _addAmount;
            _addAmount = 0;
            _addSuccess?.Invoke();
            
            _addError = null;
            _addSuccess = null;
        }

        public static void AddCoinsError()
        {
            _addAmount = 0;
            _addError?.Invoke();
            
            _addError = null;
            _addSuccess = null;
        }

        public static void WithdrawCoins(int amount, string key, Action onSuccess, Action onError)
        {
            _spentAmount = amount;
            RemoveCoins?.Invoke(amount,key);

            _withdrawSuccess = onSuccess;
            _withdrawError = onError;
        }

        public static void WithdrawError()
        {
            _spentAmount = 0;
            _withdrawError?.Invoke();
            
            _withdrawError = null;
            _withdrawSuccess = null;
        }
        
        public static void WithdrawSuccess()
        {
            CurrentBalance -= _spentAmount;
            _spentAmount = 0;
            _withdrawSuccess?.Invoke();

            _withdrawError = null;
            _withdrawSuccess = null;
        }

        public void Load(LoadData request)
        {
            CurrentBalance = request.data.coins_balance;
        }
    }
}