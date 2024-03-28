using System;
using UnityEngine;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class SculptureParkDestroyedCustomAction : CustomCutsceneAction
    {
        private readonly IceCubesService _iceCubesService;

        public SculptureParkDestroyedCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.SculptureParkDestroyed;
        }

        public override void Execute(Action action = null)
        {
            _iceCubesService.DestroyBuilding();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}
