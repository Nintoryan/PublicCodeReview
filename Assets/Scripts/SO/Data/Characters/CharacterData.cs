using UnityEngine;

namespace SO.Data.Characters
{
    [CreateAssetMenu(fileName = "Character_", menuName = "SO/Data/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        public CharacterType Type;
        public CharacterPresentationModel PresentationModel;
    }
}