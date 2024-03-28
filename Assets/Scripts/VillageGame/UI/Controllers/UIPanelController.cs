using VillageGame.UI.Panels;

namespace VillageGame.UI.Controllers
{
    public abstract class UIPanelController
    {
        protected UIPanel _uiPanel;

        public virtual void SwitchPanel() { }

        public virtual void ShowUIPanel()
        {
            _uiPanel.Show();
        }

        public virtual void HideUIPanel()
        {
            _uiPanel.Hide();
        }
    }
}
