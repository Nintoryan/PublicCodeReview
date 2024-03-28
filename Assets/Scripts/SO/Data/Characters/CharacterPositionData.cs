using System;
using UnityEngine;

namespace SO.Data.Characters
{
    [Serializable]
    public class CharacterPositionData
    {
        public CharacterType Character;
        public Vector2 CharacterPosition;
        public bool IsUp;
        public bool isLeft;
    }
}