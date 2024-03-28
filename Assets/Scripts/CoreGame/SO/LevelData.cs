using System;
using CoreGame.Data;
using UnityEngine;

namespace CoreGame.SO
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "SO/CoreGame/LevelData")]
    public class LevelData : ScriptableObject
    {
        public float LevelTime = 60;
        public int LevelReward = 1000;
        public LevelObjectContainers LevelObjectContainers;

        private void OnValidate()
        {
            for (int i = 0; i < LevelObjectContainers.ObjectsContainers.Length; i++)
            {
                LevelObjectContainers.ObjectsContainers[i].ID = LevelObjectContainers.ObjectsContainers[i].ContainerPosition.ToString();
                if (LevelObjectContainers.ObjectsContainers[i].ContainerType == ContainerType.MegaContainer)
                {
                    LevelObjectContainers.ObjectsContainers[i].ID += " MegaObject";
                }
            }
        }
    }

    [Serializable]
    public struct LevelObjectContainers
    {
        public ObjectContainer[] ObjectsContainers;

        public LevelObjectContainers Clone()
        {
            LevelObjectContainers copy = new LevelObjectContainers();
            copy.ObjectsContainers = new ObjectContainer[ObjectsContainers.Length];

            for (int i = 0; i < ObjectsContainers.Length; i++)
            {
                ObjectContainer container = ObjectsContainers[i];
                ObjectContainer containerCopy = new ObjectContainer();
                containerCopy.ID = container.ID;
                containerCopy.ContainerPosition = container.ContainerPosition;
                containerCopy.ContainerType = container.ContainerType;
                containerCopy.IsEnabled = container.IsEnabled;
                containerCopy.IsLocked = container.IsLocked;
                containerCopy.Columns = new Column[container.Columns.Length];
                for (int j = 0; j < container.Columns.Length; j++)
                {
                    Column column = container.Columns[j];
                    Column columnCopy = new Column();
                    columnCopy.Rows = new Row[column.Rows.Length];
                    for (int k = 0; k < column.Rows.Length; k++)
                    {
                        Row row = column.Rows[k];
                        Row rowCopy = new Row();
                        rowCopy.Type = row.Type;
                        rowCopy.IsLocked = row.IsLocked;
                        rowCopy.IsHided = row.IsHided;
                        rowCopy.IsPacked = row.IsPacked;
                        columnCopy.Rows[k] = rowCopy;
                    }
                    containerCopy.Columns[j] = columnCopy;
                }

                copy.ObjectsContainers[i] = containerCopy;
            }
            return copy;
        }
    }

    [Serializable]
    public struct ObjectContainer
    {
        public string ID;
        public Vector2Int ContainerPosition;
        public ContainerType ContainerType;
        public bool IsEnabled;
        public bool IsLocked;
        public Column[] Columns;
    }

    [Serializable]
    public struct Column
    {
        public Row[] Rows;
    }

    [Serializable]
    public struct Row
    {
        public ObjectType Type;
        public bool IsLocked;
        public bool IsHided;
        public bool IsPacked;
    }
}
