using System;
using System.Globalization;
using System.Threading.Tasks;
using Util;
using VContainer;
using VillageGame.Services;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;
using Web.ResponseStructs;

namespace CoreGame
{
    public class WinCounter : ILoading
    {
        private readonly CoreGameLoadService _coreGameService;
        private WinNumberData _winNumberData;
        public Action<WinNumberData> NumberOfWinCountedEvent;
        private readonly TimeObserver _timeObserver;

        public WinNumberData WinNumberData => _winNumberData;

        [Inject]
        public WinCounter(IObjectResolver objectResolver)
        {
            _coreGameService = objectResolver.Resolve<CoreGameLoadService>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
            SubscribeToEvents(true);
        }

        ~WinCounter()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _coreGameService.CoreGameWinEvent += OnCoreGameWinEvent;
                _coreGameService.CoreGameLoseEvent += OnCoreGameLoseEvent;
            }
            else
            {
                _coreGameService.CoreGameWinEvent -= OnCoreGameWinEvent;
                _coreGameService.CoreGameLoseEvent -= OnCoreGameLoseEvent;
            }
        }

        private void OnCoreGameWinEvent(int level)
        {
            _winNumberData.all_wins++;
            _winNumberData.wins_in_row++;
            NumberOfWinCountedEvent?.Invoke(_winNumberData);
        }

        private void OnCoreGameLoseEvent(int level)
        {
            _winNumberData.wins_in_row = 0;
            NumberOfWinCountedEvent?.Invoke(_winNumberData);
        }

        public void Load(LoadData request)
        {
            if (request.data.win_number != null)
            {
                if (Extensions.TryParseDate(request.data.win_number.day, out var date))
                {
                    if (date.Date.Day == _timeObserver.CurrentPlayerTime.Day)
                    {
                        _winNumberData = request.data.win_number;
                    }
                }
            }

            if (_winNumberData == null)
            {
                _winNumberData = new WinNumberData();
                _winNumberData.day = _timeObserver.CurrentPlayerTime.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}