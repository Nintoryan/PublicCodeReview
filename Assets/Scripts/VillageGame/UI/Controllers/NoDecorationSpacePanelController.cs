using VContainer;
using VillageGame.UI.Panels;

namespace VillageGame.UI.Controllers
{
    public class NoDecorationSpacePanelController
    {
        private readonly NoDecorationSpaceWarningPanel _warningPanel; 
        
        [Inject]
        public NoDecorationSpacePanelController(IObjectResolver objectResolver)
        {
            _warningPanel = objectResolver.Resolve<UIFacade>().FindWarningPanel<NoDecorationSpaceWarningPanel>();
        }

        public void ShowNoSpaceWarningPanel()
        {
            _warningPanel.Show();   
        }
    }
}