using System.Linq;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using VillageGame.Logic.Characters;

namespace SO
{
    [CreateAssetMenu(fileName = "CharactersConfig",  menuName = "SO/CharactersConfig")]
    public class CharactersConfig : ScriptableObject
    {
        public CharacterData[] Characters;

        public Character Prefab;
        public CharacterData GetCharacter(CharacterType type)
        {
            return Characters.FirstOrDefault(c => c.Type == type);
        }
    }
}