using System;
using UnityEngine;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class BigfootDisappearCustomAction : CustomCutsceneAction
    {
        private readonly IceCubesService _iceCubesService;

        public BigfootDisappearCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.BigfootDisappear;
        }

        public override void Execute(Action action = null)
        {
            _iceCubesService.DeactivateCharacter();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}
