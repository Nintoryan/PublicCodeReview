using System.Collections.Generic;
using UnityEngine;

namespace CoreGame.SO
{
    [CreateAssetMenu(fileName = "ContainerObjectDatabase", menuName = "SO/CoreGame/ContainerObjectDatabase")]
    public class ContainerObjectDatabase : ScriptableObject
    {
        public List<ContainerObjectData> ContainerObjects;
    }
}
