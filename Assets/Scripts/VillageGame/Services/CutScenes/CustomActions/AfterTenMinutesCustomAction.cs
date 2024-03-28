using System;
using VContainer;
using VillageGame.Logic.Curtains;
using VillageGame.UI;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class AfterTenMinutesCustomAction : CustomCutsceneAction
    {
        private readonly CloudCurtain _cloudCurtain;

        public AfterTenMinutesCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _cloudCurtain = objectResolver.Resolve<UIFacade>().CloudCurtain;
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.AfterTenMinutesCurtain;
        }

        public override void Execute(Action action = null)
        {
            _cloudCurtain.ShowWithText(
                text: "Спустя 10 минут\nстранствий\nпо сугробам...",
                onShown: () => action?.Invoke(),
                onHidden: () => EndCustomActionEvent?.Invoke(this));
        }
    }
}