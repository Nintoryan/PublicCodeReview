using System;
using VContainer;
using VillageGame.UI.Controllers;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class ShowEnterNameWindowCustomAction : CustomCutsceneAction
    {
        private readonly EnterNamePanelController _enterNamePanelController;

        public ShowEnterNameWindowCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _enterNamePanelController = objectResolver.Resolve<EnterNamePanelController>();
            _enterNamePanelController.SetPlayerNameEvent += SetPlayerNameEvent;
        }

        ~ShowEnterNameWindowCustomAction()
        {
            _enterNamePanelController.SetPlayerNameEvent -= SetPlayerNameEvent;
        }
        
        private void SetPlayerNameEvent(string obj)
        {
            EndCustomActionEvent?.Invoke(this);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.ShowEnterNameWindow;
        }

        public override void Execute(Action action = null)
        {
            _enterNamePanelController.Show();
        }
    }
}