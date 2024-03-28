using System;
using System.Collections.Generic;
using System.Linq;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Logic.Characters;
using Object = UnityEngine.Object;

namespace VillageGame.Services.Storages
{
    public class CharactersStorage
    {
        public List<Character> ExistingCharacters { get; } = new();
        public Action<int> AddCharacter;

        [Inject] public CharactersStorage(){}

        public void Add(Character character)
        {
            if (character.Data == null)
            {
                return;
            }

            if (ExistingCharacters.All(c => c.Data.Type != character.Data.Type))
            {
                ExistingCharacters.Add(character);

                AddCharacter?.Invoke((int)character.Data.Type);
                Debugging.Log("Character data was null wtf", ColorType.Red);
            }
            else
            {
                Object.Destroy(character.gameObject);
            }
        }

        public bool Contains(CharacterType type)
        {
            return ExistingCharacters.Any(c => c.Data.Type == type);
        }

        public Character GetCharacter(CharacterType characterType)
        {
            return ExistingCharacters.FirstOrDefault(x => x.Data?.Type == characterType);
        }
    }
}