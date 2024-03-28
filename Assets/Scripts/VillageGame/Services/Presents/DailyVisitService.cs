using System;
using System.Threading.Tasks;
using SO;
using SO.Data.Presents;
using Util;
using VContainer;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;
using Web.ResponseStructs;

namespace VillageGame.Services.Presents
{
    public class DailyVisitService : ILoading
    {
        public DailyVisitsPresentData VisitData { get; private set; }
        private readonly DailyVisitPresentsConfig _presentConfig;
        private readonly TimeObserver _timeObserver;

        public Action<PresentValueData> PressTakeDailyVisitPresentEvent;
        public Action SetVisitNumber;

        [Inject]
        public DailyVisitService(IObjectResolver objectResolver)
        {
            _presentConfig = objectResolver.Resolve<DailyVisitPresentsConfig>();
            _timeObserver = objectResolver.Resolve<TimeObserver>();
        }
        
        public void OpenLastPreset()
        {
            if (VisitData.next_present_number < _presentConfig.DailyVisitPresents.Length)
            {
                var present = _presentConfig.DailyVisitPresents[VisitData.next_present_number];
                VisitData.next_present_number++;
                VisitData.is_open_today = true;
                PressTakeDailyVisitPresentEvent?.Invoke(present);
            }
        }

        public bool IsEmpty()
        {
            return VisitData.next_present_number >= _presentConfig.DailyVisitPresents.Length;
        }
        
        public void Load(LoadData request)
        {
            VisitData = request.data.visit_presents;
            if (VisitData == null || request.data.daily_tasks == null)
            {
                VisitData = new DailyVisitsPresentData();
                VisitData.visit_number++;
                SetVisitNumber?.Invoke();
                return;
            }
            
            if (request.data.daily_tasks.date != _timeObserver.CurrentDay.GetFullDateText() || !VisitData.is_open_today)
            {
                VisitData.visit_number++;
                if (VisitData.visit_number > 8)
                {
                    VisitData.visit_number = 1;
                    VisitData.next_present_number = 0;
                }
                VisitData.is_open_today = false;
                SetVisitNumber?.Invoke();
            }
        }
    }
}