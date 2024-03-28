using System;
using UnityEngine;
using VContainer;
using VillageGame.Logic.Curtains;
using VillageGame.Services.Characters;
using VillageGame.Services.Snowdrifts;
using VillageGame.Services.Storages;


namespace VillageGame.Services.CutScenes.CustomActions
{
    public class SnowFlakeFactory_Curtains_2 : CustomCutsceneAction
    {
        private readonly BigSnowdrifts _bigSnowDrift;
        private readonly CutSceneAnimator _cutSceneAnimator;
        private readonly CharactersNavigationService _characterNavigationService;
        private readonly CharactersStorage _characterStorage;

        public SnowFlakeFactory_Curtains_2(IObjectResolver objectResolver) : base(objectResolver)
        {
            _bigSnowDrift = objectResolver.Resolve<BigSnowdrifts>();
            _cutSceneAnimator = objectResolver.Resolve<CutSceneAnimator>();
            _characterNavigationService = objectResolver.Resolve<CharactersNavigationService>();
            _characterStorage = objectResolver.Resolve<CharactersStorage>();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.SnowFlakeFactory_Curtains_2;
        }

        public override void Execute(Action action = null)
        {
            CloudCurtain.OnShownEvent += CloudCurtainOnShownEvent;
        }

        private void CloudCurtainOnShownEvent()
        {
            CloudCurtain.OnShownEvent -= CloudCurtainOnShownEvent; 
            
            _bigSnowDrift.SetSnowflakeFactoryCutsceneState(false);
            _bigSnowDrift.SetGiantSnowMan(true);
            _cutSceneAnimator.EnableTrain();
            
            foreach (var character in _characterStorage.ExistingCharacters)
            {
                var position = _characterNavigationService.CaptureChristmasTreePoint(character);
                character.TeleportToPosition(position);
                character.Move.LookTarget = Vector3.down * 10;
                character.Move.BlockMove();
            }
            
            EndCustomActionEvent?.Invoke(this);
        }
        
    }
}