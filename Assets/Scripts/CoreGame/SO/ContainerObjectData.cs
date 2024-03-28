using CoreGame.Data;
using UnityEngine;

namespace CoreGame.SO
{
    [CreateAssetMenu(fileName = "ContainerObjectData", menuName = "SO/CoreGame/ContainerObjectData")]
    public class ContainerObjectData : ScriptableObject
    {
        public Sprite Sprite;
        public ObjectType Type;
        
    }
}
