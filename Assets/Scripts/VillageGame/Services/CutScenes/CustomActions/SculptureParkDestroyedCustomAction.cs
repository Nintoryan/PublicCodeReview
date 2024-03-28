using System;
using UnityEngine;
using VContainer;

namespace VillageGame.Services.CutScenes.CustomActions
{
    public class SculptureParkRestoredCustomAction : CustomCutsceneAction
    {
        private readonly IceCubesService _iceCubesService;

        public SculptureParkRestoredCustomAction(IObjectResolver objectResolver) : base(objectResolver)
        {
            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.SculptureParkRestored;
        }

        public override void Execute(Action action = null)
        {
            _iceCubesService.RestoreBuilding();
            EndCustomActionEvent?.Invoke(this);
        }
    }
}
