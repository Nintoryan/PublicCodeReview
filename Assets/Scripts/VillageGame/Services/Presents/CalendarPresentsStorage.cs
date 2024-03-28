using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SO;
using SO.Data;
using SO.Data.Presents;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Services.CutScenes;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;
using Web.ResponseStructs.PayloadValues;

namespace VillageGame.Services.Presents
{
    public class CalendarPresentsStorage : ILoading
    {
        private readonly CutSceneController _cutSceneController;
        public List<OpenedPresentBoxData> OpenedPresentsData { get; } = new();

        public Action<List<OpenedPresentBoxData>, PresentBoxData> OpenCalendarPresentEvent;
        public Action<PresentBoxData> TakeNewClosedPresentBoxEvent;

        private PresentBoxesConfig _presentBoxesConfig;

        [Inject]
        public CalendarPresentsStorage(IObjectResolver objectResolver)
        {
            _cutSceneController = objectResolver.Resolve<CutSceneController>();
            _presentBoxesConfig = objectResolver.Resolve<PresentBoxesConfig>();
            SubscribeToEvents(true);
        }

        ~CalendarPresentsStorage()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _cutSceneController.EndCutSceneEvent += EndCutSceneEvent;

                PromoCodeService.PromoCodeSuccess += OnPromoCodeIssued;
            }
            else
            {
                _cutSceneController.EndCutSceneEvent -= EndCutSceneEvent;
                
                PromoCodeService.PromoCodeSuccess -= OnPromoCodeIssued;
            }
        }
        
        private void EndCutSceneEvent(CutSceneData cutSceneData)
        {
            if (cutSceneData.PresentBox == null)
            {
                return;
            }

            AddPresentBox(cutSceneData.PresentBox);
        }

        public bool IsHas(DayData openDay)
        {
            return OpenedPresentsData.Any(p => p.Date.Equals(openDay));
        }

        public bool IsOpen(DayData openDay)
        {
            return OpenedPresentsData.Any(p => p.Date.Equals(openDay) && p.IsOpen);
        }

        public void OpenPresentBox(PresentBoxData presentBox)
        {
            var presentData = OpenedPresentsData.FirstOrDefault(p => p.Date.Equals(presentBox.OpenDate));

            if (presentData != null)
            {
                presentData.IsOpen = true;
                OpenCalendarPresentEvent?.Invoke(OpenedPresentsData, presentBox);
            }
        }

        public OpenedPresentBoxData GetOpenedPromoCode(PresentBoxData presentBoxData)
        {
            if (presentBoxData.PresentValue is PromocodePresentValueData promoData)
            {
                var opened = OpenedPresentsData.FirstOrDefault(o => o.ID == promoData.PromocodeID);
                return opened;
            }
            return null;
        }
        
        private void OnPromoCodeIssued(int promoID, string promocode)
        {
            var presentBox = _presentBoxesConfig.PresentsBoxesData.FirstOrDefault(b
                => b.PresentValue is PromocodePresentValueData data && data.PromocodeID == promoID);
            
            var presentData = OpenedPresentsData.FirstOrDefault(d => d.ID == promoID);

            if (presentData == null || presentBox == null)
            {
                Debugging.Log($"Couldn't find present fro promoID {promoID}");
                return;
            }
            
            presentData.IsOpen = true;
            presentData.StringValue = promocode;
            
            Debugging.Log($"Promo issued and saved! {promoID}");
            OpenCalendarPresentEvent?.Invoke(OpenedPresentsData, presentBox);

        }

        public void OpenPresentBox(PresentValueData presentValue)
        {
            var presentBox = _presentBoxesConfig.PresentsBoxesData.FirstOrDefault(b => b.PresentValue.Equivalent(presentValue));
            if (presentBox == null) return;
            
            var presentData = OpenedPresentsData.FirstOrDefault(p => p.Date.Equals(presentBox.OpenDate));
            if (presentData == null) return;
            
            presentData.IsOpen = true;
            
            Debugging.Log($"CalendarPresentsStorage: OpenPresentBox {presentBox.OpenDate.GetDateText()}", ColorType.Blue);
            
            OpenCalendarPresentEvent?.Invoke(OpenedPresentsData, presentBox);
        }

        private void AddPresentBox(PresentBoxData present)
        {
            if (OpenedPresentsData.All(p => p.ID != present.ID))
            {
                var presentData = new OpenedPresentBoxData
                {
                    ID = present.ID,
                    Date = present.OpenDate,
                    IsOpen = false
                };

                OpenedPresentsData.Add(presentData);
                TakeNewClosedPresentBoxEvent?.Invoke(present);
            }
        }


        public void Load(LoadData request)
        {
            if (request.data.opened_presents != null)
            {
                var presents = request.data.opened_presents;
                foreach (var presentData in presents.Where(presentData => !presentData.Date.IsEmpty))
                {
                    OpenedPresentsData.Add(presentData);
                }
            }
        }
    }
}