using System;
using System.Linq;
using System.Collections.Generic;
using CoreGame.SO;
using CoreGame.Data;
using UnityEngine;
using Util;

namespace CoreGame
{
    [Serializable]
    public class LevelStorageModel
    {
        public LevelObjectContainers LevelObjectContrainers = new();
        [HideInInspector] public bool IsBackLineActive;
        private ObjectType _megaContainerObjectId;

        public LevelStorageModel(RandomizedLevelData levelData)
        {
            if (levelData.MegaContainers.Count != 0)
            {
                _megaContainerObjectId = levelData.MegaContainers[0].ObjectType;
            }
            IsBackLineActive = levelData.IsBackLineActive;
            if (levelData.IsFinal)
            {
                LevelObjectContrainers = levelData.StableLevelObjectContainers.Clone();
            }
            else
            {
                LevelEmptyObjectContainers levelEmptyObjectContrainers = levelData.LevelEmptyObjectContainers.Clone();
                var levelConrainers = new List<ObjectContainer>();

                foreach (var objectContainer in levelEmptyObjectContrainers.ObjectEmptyContainers)
                {
                    Column[] columns;
                    switch (objectContainer.ContainerType)
                    {
                        case ContainerType.MegaContainer:
                            columns = new Column[levelData.MegaContainerColumns];
                            break;
                        case ContainerType.SuperMegaContainer:
                            columns = new Column[levelData.SuperMegaContainerColumns];
                            break;
                        default:
                            columns = new Column[levelData.Columns];
                            break;
                    }

                    for (int j = 0; j < columns.Length; j++)
                    {
                        var rowsObjects = new Row[levelData.Rows];
                        columns[j].Rows = rowsObjects;
                    }

                    ObjectContainer container = new ObjectContainer()
                    {
                        ID = objectContainer.ID,
                        ContainerPosition = objectContainer.ContainerPosition,
                        ContainerType = objectContainer.ContainerType,
                        IsEnabled = objectContainer.IsEnabled,
                        IsLocked = objectContainer.IsLocked,
                        Columns = columns
                    };
                    levelConrainers.Add(container);
                }

                LevelObjectContrainers.ObjectsContainers = levelConrainers.ToArray();
            }
        }

        public ObjectContainer FindContainerByPosition(Vector2Int containerPosition)
        {
            return LevelObjectContrainers.ObjectsContainers.FirstOrDefault(container =>
                container.ContainerPosition == containerPosition);
        }

        public Row FindObjectByPosition(Vector2Int containerPosition, Vector2Int objectPosition)
        {
            ObjectContainer objectContainer = FindContainerByPosition(containerPosition);
            return objectContainer.Columns[objectPosition.x].Rows[objectPosition.y];
        }

        public void SetPacked(bool isPacked, Vector2Int containerPosition, Vector2Int objectPosition)
        {
            ObjectContainer objectContainer = FindContainerByPosition(containerPosition);
            objectContainer.Columns[objectPosition.x].Rows[objectPosition.y].IsPacked = isPacked;
        }

        public void SetContainerContent(Vector2Int containerPosition, Vector2Int contentPosition, ObjectType newId, bool isLocked, bool IsHided, bool isPacked)
        {
            ObjectContainer matchingContainer = FindContainerByPosition(containerPosition);
            matchingContainer.Columns[contentPosition.x].Rows[contentPosition.y].Type = newId;
            matchingContainer.Columns[contentPosition.x].Rows[contentPosition.y].IsLocked = isLocked;
            matchingContainer.Columns[contentPosition.x].Rows[contentPosition.y].IsHided = IsHided;
            matchingContainer.Columns[contentPosition.x].Rows[contentPosition.y].IsPacked = isPacked;
        }

        public bool CheckAllRowsAreEmpty()
        {
            bool isEmpty = !LevelObjectContrainers.ObjectsContainers
                .SelectMany(container => container.Columns)
                .SelectMany(column => column.Rows)
                .Any(row => row.Type != ObjectType.None);

            return isEmpty;
        }

        public List<LevelPositionData> GetPositionsData(bool isEmptyPositions, bool isBackLineActive, params ContainerType[] containerTypes)
        {
            List<LevelPositionData> positions = new();
            foreach (var objectContainer in LevelObjectContrainers.ObjectsContainers)
            {
                if (objectContainer.IsEnabled && (containerTypes.Length == 0 || containerTypes.Contains(objectContainer.ContainerType)))
                {
                    for (int j = 0; j < objectContainer.Columns.Length; j++)
                    {
                        if (isBackLineActive)
                        {
                            for (int k = 0; k < objectContainer.Columns[j].Rows.Length; k++)
                            {
                                if (isEmptyPositions)
                                {
                                    if (objectContainer.Columns[j].Rows[k].Type <= 0)
                                    {
                                        positions.Add(new LevelPositionData(objectContainer.ContainerPosition, new Vector2Int(j, k)));
                                    }
                                }
                                else
                                {
                                    if (
                                        objectContainer.Columns[j].Rows[k].Type > 0 && 
                                        !objectContainer.Columns[j].Rows[k].IsLocked && 
                                        objectContainer.Columns[j].Rows[k].Type != _megaContainerObjectId
                                        )
                                    {
                                        positions.Add(new LevelPositionData(objectContainer.ContainerPosition, new Vector2Int(j, k)));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isEmptyPositions)
                            {
                                if (objectContainer.Columns[j].Rows[0].Type <= 0)
                                {
                                    positions.Add(new LevelPositionData(objectContainer.ContainerPosition, new Vector2Int(j, 0)));
                                }
                            }
                            else
                            {
                                if (objectContainer.Columns[j].Rows[0].Type > 0 && !objectContainer.Columns[j].Rows[0].IsLocked)
                                {
                                    positions.Add(new LevelPositionData(objectContainer.ContainerPosition, new Vector2Int(j, 0)));
                                }
                            }
                        }
                    }
                }
            }
            return positions;
        }
    }

    public class LevelPositionData
    {
        public Vector2Int ContainerPosition;
        public Vector2Int ObjectPosition;

        public LevelPositionData(Vector2Int containerPosition, Vector2Int objectPosition)
        {
            ContainerPosition = containerPosition;
            ObjectPosition = objectPosition;
        }
    }
}
