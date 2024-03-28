using System;
using SO.Data.Presents;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Services.Presents;
using VillageGame.UI.Buttons;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Controllers
{
    public class PresentInformationPanelController : IInitializable
    {
        private readonly CalendarPanelController _calendarController;
        private readonly PresentInformationPanel _presentInformation;
        private readonly CalendarPresentsStorage _calendarStorage;

        public Action<PresentValueData> GetPresentEvent;

        [Inject]
        public PresentInformationPanelController(IObjectResolver objectResolver)
        {
            _presentInformation = objectResolver.Resolve<UIFacade>().FindPanel<PresentInformationPanel>();
            _calendarController = objectResolver.Resolve<CalendarPanelController>();
            _calendarStorage = objectResolver.Resolve<CalendarPresentsStorage>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }


        ~PresentInformationPanelController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _calendarController.ShowPresentInfo += ShowPanel;
                _presentInformation.ClaimPresent += ClaimPresent;
            }
            else
            {
                _calendarController.ShowPresentInfo -= ShowPanel;
                _presentInformation.ClaimPresent -= ClaimPresent;
            }
        }
        

        private void ClaimPresent(PresentValueData presentValueData)
        {
            Debugging.Log($"2. PresentInformationPanel: OnPressGetPresentButton", ColorType.Blue);
            
            _calendarStorage.OpenPresentBox(presentValueData);
            GetPresentEvent?.Invoke(presentValueData);
        }

        private void ShowPanel(CalendarPresentButton boxData)
        {
            Debugging.Log("PresentPanelController -> PressPresentButton", ColorType.Blue);
            
            _presentInformation.SetPresentData(
                boxData.Data.PresentValue,
                _calendarStorage.GetOpenedPromoCode(boxData.Data),
                boxData.IsOpen);
            
            _presentInformation.Show();

            if (boxData.Data.PresentValue is not DodoCoinsPresentValueData && !boxData.IsOpen)
            {
                _calendarStorage.OpenPresentBox(boxData.Data.PresentValue);
                GetPresentEvent?.Invoke(boxData.Data.PresentValue);
            }
        }
    }
}