using System;
using UnityEngine;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class BigfootAppearCustomAction : CustomCutsceneAction
    {
        private readonly IceCubesService _iceCubesService;

        public BigfootAppearCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.BigfootAppear;
        }

        public override void Execute(Action action = null)
        {
            _iceCubesService.ActivateCharacter();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}
