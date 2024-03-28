using VContainer;
using VContainer.Unity;
using VillageGame.UI.Panels;
using VillageGame.UI.Panels.Warnings;

namespace VillageGame.UI.Controllers
{
    public class EnergyWarningPanelController : IInitializable
    {
        private readonly EnergyWarningUIPanel _warningPanel;

        [Inject]
        public EnergyWarningPanelController(IObjectResolver resolver)
        {
            _warningPanel = resolver.Resolve<UIFacade>().FindPanel<EnergyWarningUIPanel>();
        }
        
        public void Initialize() { }
        
        public void ActivateWarning()
        {
            _warningPanel.Show();
        }
    }
}
