using SO;
using SO.Data;
using SO.Data.Characters;
using VContainer;
using VillageGame.Data.Types;
using VillageGame.Infrastructure.Factories;
using VillageGame.Logic.Characters;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Storages;
using Web.RequestStructs;

namespace VillageGame.Services.Characters
{
    public class CharacterSpawnService : ILoading
    {
        private readonly CharactersFactory _charactersFactory;
        private readonly CharactersStorage _characterStorage;
        private readonly BuildingOnMapStorage _buildingOnMapStorage;
        private readonly BuildingsConfig _buildingsConfig;

        [Inject]
        public CharacterSpawnService(IObjectResolver objectResolver)
        {
            _charactersFactory = objectResolver.Resolve<CharactersFactory>();
            _characterStorage = objectResolver.Resolve<CharactersStorage>();
            _buildingOnMapStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _buildingsConfig = objectResolver.Resolve<BuildingsConfig>();

            SubscribeToEvents(true);
        }

        ~CharacterSpawnService()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _buildingOnMapStorage.BuildBuildingEvent += OnBuildHouse;
            }
            else
            {
                _buildingOnMapStorage.BuildBuildingEvent -= OnBuildHouse;
            }
        }

        private void OnBuildHouse(BuildingType buildingType, int buildingId)
        {
            if (buildingType != BuildingType.House)
            {
                return;
            }

            if (_buildingOnMapStorage.TryGetBuilding(buildingType, buildingId, out var building))
            {
                var characters = building.Data.Characters;

                foreach (var characterType in characters)
                {
                    var createdCharacter = _characterStorage.GetCharacter(characterType);
                    if (createdCharacter == null)
                    {
                        var character = _charactersFactory.CreateCharacter(characterType, buildingId);
                        _characterStorage.Add(character);
                    }
                }
            }
        }

        public void Load(LoadData request)
        {
            if (request.data == null || request.data.existing_characters == null)
            {
                _characterStorage.Add(LoadCharacter(CharacterType.Guide));
                _characterStorage.Add(LoadCharacter(CharacterType.Grandma));
                _characterStorage.Add(LoadCharacter(CharacterType.Grandpa));
                return;
            }

            foreach (var characterType in request.data.existing_characters)
            {
                _characterStorage.Add(LoadCharacter(characterType));
            }

            if (!_characterStorage.Contains(CharacterType.Guide))
            {
                _characterStorage.Add(LoadCharacter(CharacterType.Guide));
            }
        }

        private Character LoadCharacter(CharacterType type)
        {
            var buildingData = _buildingsConfig.GetData(type);
            return _charactersFactory.CreateCharacterAtRandomPoint(type, buildingData!= null ? buildingData.ID : 0);
        }
    }
}