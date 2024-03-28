using CoreGame.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreGame.SO
{
    [CreateAssetMenu(fileName = "RandomizedLevelData", menuName = "SO/CoreGame/RandomizedLevelData")]
    public class RandomizedLevelData : ScriptableObject
    {
        public float LevelTime = 60;
        public int LevelReward = 1000;
        [Header("Расположение активных контейнеров на уровне 7x3" +
            "\n Мегаконтейнер на какой-либо строке автоматически деактивирует два крайних контейнера")]
        public ArrayLayout ArrayLayout;

        [Header("На уровне заспаунится по 3 предмета за каждую присутствующую " +
            "\n в списке запись. Если надо заспаунить 6 предметов, то можно добавить " +
            "\n две одинаковые записи")]
        public ObjectType[] ObjectsTypes;

        public bool IsBackLineActive;
        public bool IsNextLevelButtonDeactivated;

        [Header("На уровне будет столько мегаконтейнеров, сколько записей в этом списке" +
            "\n RowId  указывает в какой строке будет находиться мегаконтейнер, снизу вверх, начиная с 0" +
            "\n Два мегаконтейнера на одной строке установить нельзя" +
            "\n ObjectType указывает на то, какой тип объектов будет связан с этим контейнером" +
            "\n Этот тип не должен повторяться в списке обычных объектов, других мегаконтейнеров " +
            "\n и снеговика")]
        public List<MegaContainerData> MegaContainers;

        [Header("Каждая запись в этом списке делает соответствующий контейнер заблокированным" +
            "\n ID указывает на то какой по порядку контейнер будет заблокирован снизу-вверх " +
            "\n и слево-направо из расчета уровня 7x3" +
            "\n UnlockCombinations указывает на то, сколько комбинаций надо собрать чтобы " +
            "\n разблокировать контейнер")]
        public List<LockedContainerData> LockedContainers;

        [Header("Количество объектов у которых видно только силуэт")]
        public int HiddenObjects = 0;
        [Header("Количество комплектов запакованных объектов. Каждый комплект - 3 объекта")]
        public int PackedComplects = 0;
        [Header("Количество комплектов цветных шаров. Каждый комплект - 3 шара")]
        public int ColorBallsComplects = 0;
        [Header("Ссылка на конфиг с голодным персонажем. " +
            "\n Если она пустая, то голодного персонажа на уровне не будет" +
            "\n Если не пустая, то настройки берутся из конфига голодного персонажа")]
        public HungryPersonData HungryPerson;
        public TutorialData Tutorial;

        [HideInInspector] 
        public LevelEmptyObjectContainers LevelEmptyObjectContainers = new LevelEmptyObjectContainers();
        public readonly int Columns = 3;
        public readonly int MegaContainerColumns = 5;
        public readonly int SuperMegaContainerColumns = 9;
        public readonly int Rows = 2;

        [Header("Является ли уровень финальным или нет" +
            "\n Пока он не отмечен - уровень будет генерироваться случайно при каждом запуске" +
            "\n Если отмечен, то последнее стартовое состояние уровня зафиксируется")]
        public bool IsFinal;
        [HideInInspector]
        public LevelObjectContainers StableLevelObjectContainers = new LevelObjectContainers();

        private void Awake()
        {
            if (LevelEmptyObjectContainers.ObjectEmptyContainers == null)
            {
                LevelEmptyObjectContainers.Initialize();
            }
        }

        private void OnValidate()
        {
            ObjectEmptyContainer[] objectEmptyContainers = LevelEmptyObjectContainers.ObjectEmptyContainers;
            for (int i = 0; i < objectEmptyContainers.Length; i++)
            {
                objectEmptyContainers[i].ID = LevelEmptyObjectContainers.ObjectEmptyContainers[i].ContainerPosition.ToString();
                objectEmptyContainers[i].ContainerType = ContainerType.Container;
                objectEmptyContainers[i].IsLocked = false;
                objectEmptyContainers[i].IsEnabled = ArrayLayout.Rows[i / 3].Row[i % 3];
            }

            for (int i = 0; i < objectEmptyContainers.Length; i++)
            {

                if (LockedContainers.Any(x => x.ID == i))
                {
                    objectEmptyContainers[i].IsLocked = true;
                    objectEmptyContainers[i].ID += " Locked";
                }

                if (MegaContainers.Any(x => x.RowId == objectEmptyContainers[i].ContainerPosition.y))
                {
                    if (objectEmptyContainers[i].ContainerPosition.x == 0)
                    {
                        MegaContainerData megaContainer = MegaContainers.First(x => x.RowId == objectEmptyContainers[i].ContainerPosition.y);
                        if (megaContainer.IsSuperMegaObject)
                        {
                            objectEmptyContainers[i].ID += " SuperMegaObject";
                            objectEmptyContainers[i].ContainerType = ContainerType.SuperMegaContainer;
                        }
                        else
                        {
                            objectEmptyContainers[i].ID += " MegaObject";
                            objectEmptyContainers[i].ContainerType = ContainerType.MegaContainer;
                        }
                    }
                    else
                    {
                        objectEmptyContainers[i].IsEnabled = false;
                    }
                }
            }
        }
    }

    [Serializable]
    public class LevelEmptyObjectContainers
    {
        public ObjectEmptyContainer[] ObjectEmptyContainers;

        public LevelEmptyObjectContainers Clone()
        {
            LevelEmptyObjectContainers copy = new LevelEmptyObjectContainers();
            copy.ObjectEmptyContainers = new ObjectEmptyContainer[ObjectEmptyContainers.Length];

            for (int i = 0; i < ObjectEmptyContainers.Length; i++)
            {
                ObjectEmptyContainer container = ObjectEmptyContainers[i];
                ObjectEmptyContainer containerCopy = new ObjectEmptyContainer(
                    container.ID,
                    container.ContainerPosition,
                    container.ContainerType,
                    container.IsEnabled,
                    container.IsLocked
                    );
                copy.ObjectEmptyContainers[i] = containerCopy;
            }

            return copy;
        }

        public LevelObjectContainers Convert(LevelObjectContainers levelObjectContainers)
        {
            LevelObjectContainers copy = new LevelObjectContainers();
            copy.ObjectsContainers = new ObjectContainer[levelObjectContainers.ObjectsContainers.Length];
            for (int i = 0; i < levelObjectContainers.ObjectsContainers.Length; i++)
            {
                ObjectContainer container = levelObjectContainers.ObjectsContainers[i];
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

        private static readonly Vector2Int[] ContainerPositions = new Vector2Int[21]
        {
            new Vector2Int(-1, 0),new Vector2Int(0, 0),new Vector2Int(1, 0),
            new Vector2Int(-1, 1),new Vector2Int(0, 1),new Vector2Int(1, 1),
            new Vector2Int(-1, 2),new Vector2Int(0, 2),new Vector2Int(1, 2),
            new Vector2Int(-1, 3),new Vector2Int(0, 3),new Vector2Int(1, 3),
            new Vector2Int(-1, 4),new Vector2Int(0, 4),new Vector2Int(1, 4),
            new Vector2Int(-1, 5),new Vector2Int(0, 5),new Vector2Int(1, 5),
            new Vector2Int(-1, 6),new Vector2Int(0, 6),new Vector2Int(1, 6)
        };

        public LevelEmptyObjectContainers()
        {
            if (ObjectEmptyContainers == null)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            ObjectEmptyContainers = new ObjectEmptyContainer[ContainerPositions.Length];
            for (int i = 0; i < ContainerPositions.Length; i++)
            {
                ObjectEmptyContainers[i] = new ObjectEmptyContainer(
                    ContainerPositions[i].ToString(),
                    ContainerPositions[i],
                    ContainerType.Container,
                    true,
                    false
                );
            }
        }
    }

    [Serializable]
    public struct ObjectEmptyContainer
    {
        public string ID;
        public Vector2Int ContainerPosition;
        [HideInInspector] public ContainerType ContainerType;
        public bool IsEnabled;
        [HideInInspector] public bool IsLocked;

        public ObjectEmptyContainer(string iD, Vector2Int containerPosition, ContainerType containerType, bool isEnabled, bool isLocked)
        {
            ID = iD;
            ContainerPosition = containerPosition;
            ContainerType = containerType;
            IsEnabled = isEnabled;
            IsLocked = isLocked;
        }
    }

    [Serializable]
    public struct MegaContainerData
    {
        public int RowId;
        public ObjectType ObjectType;
        public bool IsSuperMegaObject;
    }

    [Serializable]
    public struct LockedContainerData
    {
        public int ID;
        public int UnlockCombinations;

        public LockedContainerData(int iD, int unlockCombinations)
        {
            ID = iD;
            UnlockCombinations = unlockCombinations;
        }
    }
}