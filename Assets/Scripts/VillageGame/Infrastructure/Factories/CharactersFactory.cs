using SO;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Logic.Characters;
using VillageGame.Logic.Tree;
using VillageGame.Services.Buildings;
using VillageGame.Services.Characters;

namespace VillageGame.Infrastructure.Factories
{
    public class CharactersFactory
    {
        private readonly CharactersConfig _characterConfig;
        private readonly CharactersNavigationService _navigationService;
        private readonly TileAreasService _tileAreasService;

        private readonly Transform _charactersRoot;
        private readonly Transform _charactersInitialPoint;
        private readonly ChristmasTree _christmasTree;
        
        [Inject]
        public CharactersFactory(IObjectResolver objectResolver)
        {
            _charactersRoot = GameObject.FindWithTag(Constance.CHARACTER_ROOT_TAG).transform;
            _charactersInitialPoint = GameObject.FindWithTag(Constance.CHARACTER_INITIAL_POINT_TAG).transform;
            _characterConfig = objectResolver.Resolve<CharactersConfig>();
            _navigationService = objectResolver.Resolve<CharactersNavigationService>();
            _tileAreasService = objectResolver.Resolve<TileAreasService>();
            _christmasTree = objectResolver.Resolve<ChristmasTree>();
        }

        public Character CreateCharacter(CharacterType type, int buildingId)
        {
            var characterData = _characterConfig.GetCharacter(type);
            var character = Object.Instantiate(_characterConfig.Prefab, _charactersInitialPoint.position, Quaternion.identity, _charactersRoot);
            character.Init(characterData, buildingId, _navigationService,_christmasTree);
            character.gameObject.name += $"_{type}_{(int)type}";
            return character;
        }

        public Character CreateCharacterAtRandomPoint(CharacterType type, int buildingId)
        {
            var characterData = _characterConfig.GetCharacter(type);
            var character = Object.Instantiate(_characterConfig.Prefab, _tileAreasService.GetRandomCharacterSpawnCellPosition(), Quaternion.identity, _charactersRoot);
            character.Init(characterData, buildingId, _navigationService,_christmasTree);
            character.gameObject.name += $"_{type}_{(int)type}";
            return character;
        }
    }
}