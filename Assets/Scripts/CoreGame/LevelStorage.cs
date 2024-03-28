using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using CoreGame.SO;
using CoreGame.Data;
using VContainer;
using SO;
using Data.Scripts.Audio;
using VillageGame.Services.Storages;
using CoreGame.UI.Panels;

namespace CoreGame
{
    public class LevelStorage : MonoBehaviour
    {
        [SerializeField] private LevelStorageModel _model;

        [SerializeField] private GridController _gridController;
        [SerializeField] private TutorialService _tutorialService;
        [SerializeField] private HungryPerson _hungryPerson;
        [SerializeField] private ContainerObjectDatabase _containerObjectDatabase;
        [SerializeField] private MainUIPanel _mainUIPanel;

        private LevelDatabase _levelDatabase;
        private CurrencyStorage _currencyStorage;

        private int _selectedLevel;
        private int _realSelectedLevel;

        private int _completedCombinations;
        private List<ObjectType> _hungryPersonObjects = new();

        [SerializeField] private int _dodoBirdSpawnLevel;
        [SerializeField] private float _dodoBirdSpawnChance;

        private List<ObjectType> _megaObjectTypes = new();
        private bool _isTutorialActive;

        public Action<int, int, bool> GameWonEvent;
        public static Action<bool, int> CoreGameWonEvent;

        [Inject]
        public void Contruct(IObjectResolver objectResolver)
        {
            _levelDatabase = objectResolver.Resolve<LevelDatabase>();
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
        }

        private void Start()
        {
            _selectedLevel = StaticPrefs.LastStartedCoreGameLevel;

            _model = new(_levelDatabase.Levels[_selectedLevel]);
            SetRandomizeMap(_levelDatabase.Levels[_selectedLevel]);
        }

        private void SetRandomizeMap(RandomizedLevelData randomizedLevelData)
        {
            if (randomizedLevelData.Tutorial != null)
            {
                _isTutorialActive = true;
            }

            if (randomizedLevelData.HungryPerson != null)
            {
                _hungryPersonObjects.AddRange(randomizedLevelData.HungryPerson.RequiredObjects);
            }
            else
            {
                if (!_isTutorialActive)
                {
                    _gridController.ShiftGridPosition();
                }
            }

            foreach (ObjectContainer objectContainer in _model.LevelObjectContrainers.ObjectsContainers)
            {
                int lockedContainerCounter = 0;
                int id = objectContainer.ContainerPosition.y * randomizedLevelData.Columns + objectContainer.ContainerPosition.x + 1;
                if (randomizedLevelData.LockedContainers.Any(x => x.ID == id))
                {
                    lockedContainerCounter = randomizedLevelData.LockedContainers.First(x => x.ID == id).UnlockCombinations;
                }

                _gridController.AddGridObject(
                    objectContainer.ContainerPosition,
                    objectContainer.ContainerType,
                    objectContainer.IsEnabled,
                    objectContainer.IsLocked,
                    lockedContainerCounter
                    );
            }

            if (_isTutorialActive)
            {
                _tutorialService.StartTutorial(randomizedLevelData.Tutorial);
            }

            if (!randomizedLevelData.IsFinal)
            {
                List<LevelPositionData> emptyPositions = _model.GetPositionsData(true, randomizedLevelData.IsBackLineActive, ContainerType.Container);

                if (randomizedLevelData.MegaContainers != null && randomizedLevelData.MegaContainers.Count > 0)
                {
                    foreach (var megaContainer in randomizedLevelData.MegaContainers)
                    {
                        _megaObjectTypes.Add(megaContainer.ObjectType);
                        ObjectContainer container = _model.LevelObjectContrainers.ObjectsContainers.First(
                            x => x.ContainerPosition.y == megaContainer.RowId &&
                            x.ContainerPosition.x == 0
                            );

                        int containerSize =
                            container.ContainerType == ContainerType.SuperMegaContainer ?
                            randomizedLevelData.SuperMegaContainerColumns :
                            randomizedLevelData.MegaContainerColumns;
                        Vector2Int contentPosition = new Vector2Int(Random.Range(0, containerSize), 0);
                        AddObject(container.ContainerPosition, contentPosition, megaContainer.ObjectType, true, false, false);

                        var megaObjectTypes = new List<ObjectType>();

                        int columnsCount;
                        if (megaContainer.IsSuperMegaObject)
                        {
                            columnsCount = randomizedLevelData.SuperMegaContainerColumns - 1;
                        }
                        else
                        {
                            columnsCount = randomizedLevelData.MegaContainerColumns - 1;
                        }

                        for (int j = 0; j < columnsCount; j++)
                        {
                            megaObjectTypes.Add(megaContainer.ObjectType);
                        }

                        AddObjectsToPositions(megaObjectTypes, emptyPositions);
                    }

                    List<LevelPositionData> megaContainerPositions = _model.GetPositionsData(true, randomizedLevelData.IsBackLineActive, ContainerType.MegaContainer, ContainerType.SuperMegaContainer);
                    emptyPositions.AddRange(megaContainerPositions);
                }

                var objectTypes = new List<ObjectType>();
                if (randomizedLevelData.ObjectsTypes != null)
                {
                    foreach (var objectType in randomizedLevelData.ObjectsTypes)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            objectTypes.Add(objectType);
                        }
                    }
                }

                if (randomizedLevelData.ColorBallsComplects > 0)
                {
                    for (int i = 0; i < randomizedLevelData.ColorBallsComplects; i++)
                    {
                        objectTypes.Add(ObjectType.RedBall);
                        objectTypes.Add(ObjectType.GreenBall);
                        objectTypes.Add(ObjectType.BlueBall);
                    }
                }

                AddObjectsToPositions(objectTypes, emptyPositions);

                if (randomizedLevelData.HungryPerson != null)
                {
                    AddObjectsToPositions(_levelDatabase.Levels[_selectedLevel].HungryPerson.RequiredObjects, emptyPositions);
                }

                CheckAllBackgroundItems();

                HideRandomObjects(_levelDatabase.Levels[_selectedLevel].HiddenObjects);

                PackRandomObjects(_levelDatabase.Levels[_selectedLevel].PackedComplects);

                randomizedLevelData.StableLevelObjectContainers.ObjectsContainers = new ObjectContainer[randomizedLevelData.LevelEmptyObjectContainers.ObjectEmptyContainers.Length];

                randomizedLevelData.StableLevelObjectContainers = randomizedLevelData.LevelEmptyObjectContainers.Convert(_model.LevelObjectContrainers);
            }
            else
            {
                bool isDodoSpawned = false;
                ObjectType dodoObjectId = ObjectType.None;
                if (_selectedLevel >= _dodoBirdSpawnLevel)
                {
                    float randomValue = Random.Range(0f, 1f);
                    if (randomValue <= _dodoBirdSpawnChance)
                    {
                        isDodoSpawned = true;
                        int randomObjectId = Random.Range(0, randomizedLevelData.ObjectsTypes.Length);
                        dodoObjectId = randomizedLevelData.ObjectsTypes[randomObjectId];
                    }
                }

                int spawnedBirds = 0;

                foreach (var container in _model.LevelObjectContrainers.ObjectsContainers)
                {
                    for (int j = 0; j < container.Columns.Length; j++)
                    {
                        Column column = container.Columns[j];
                        for (int k = 0; k < column.Rows.Length; k++)
                        {
                            Row row = column.Rows[k];
                            if (row.Type > 0)
                            {
                                if (isDodoSpawned)
                                {
                                    if (row.Type == dodoObjectId && spawnedBirds < 3)
                                    {
                                        row.Type = ObjectType.Bird;
                                        spawnedBirds++;
                                    }
                                }
                                AddObject(container.ContainerPosition, new Vector2Int(j, k), row.Type, row.IsLocked, row.IsHided, row.IsPacked);
                            }
                        }
                    }
                }

                foreach (var megaContainer in randomizedLevelData.MegaContainers)
                {
                    _megaObjectTypes.Add(megaContainer.ObjectType);
                }
            }

            if (randomizedLevelData.HungryPerson != null)
            {
                _hungryPerson.ActivatePerson();
                _hungryPerson.SetRequiredObjectSprite(
                    _containerObjectDatabase.ContainerObjects
                    .First(x => x.Type == _levelDatabase.Levels[_selectedLevel].HungryPerson.RequiredObjects[0]).Sprite
                    );
            }
            else
            {
                _hungryPerson.DeactivatePerson();
            }
        }

        private void PackRandomObjects(int complectsAmount)
        {
            for (int i = 0; i < complectsAmount; i++)
            {
                List<(Vector2Int containerPosition, Vector2Int contentPosition, ObjectType type)> possibleObjects = new();
                foreach (var objectContainer in _model.LevelObjectContrainers.ObjectsContainers)
                {
                    for (int k = 0; k < objectContainer.Columns.Length; k++)
                    {
                        for (int l = 0; l < objectContainer.Columns[k].Rows.Length; l++)
                        {
                            Row row = objectContainer.Columns[k].Rows[l];
                            if (
                                row.Type > 0 &&
                                !row.IsLocked &&
                                !row.IsHided &&
                                !objectContainer.IsLocked &&
                                objectContainer.ContainerType != ContainerType.MegaContainer &&
                                objectContainer.ContainerType != ContainerType.SuperMegaContainer &&
                                row.Type != ObjectType.RedBall &&
                                row.Type != ObjectType.GreenBall &&
                                row.Type != ObjectType.BlueBall
                                )
                            {
                                possibleObjects.Add((objectContainer.ContainerPosition, new Vector2Int(k, l), objectContainer.Columns[k].Rows[l].Type));
                            }
                        }
                    }
                }

                List<ObjectType> uniqueTypes = possibleObjects.Select(obj => obj.type).Distinct().ToList();
                if (uniqueTypes.Count >= 3)
                {
                    List<ObjectType> randomTypes = uniqueTypes.OrderBy(x => Guid.NewGuid()).Take(3).ToList();

                    foreach (ObjectType type in randomTypes)
                    {
                        List<(Vector2Int, Vector2Int, ObjectType)> objectsOfType = possibleObjects.Where(obj => obj.type == type).ToList();

                        (Vector2Int containerPosition, Vector2Int contentPosition, ObjectType type) selectedObject = objectsOfType.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                        _model.SetPacked(true, selectedObject.containerPosition, selectedObject.contentPosition);
                        _gridController.PackSubgridObject(selectedObject.containerPosition, selectedObject.contentPosition);
                    }
                }
            }
        }

        private void HideRandomObjects(int amount)
        {
            List<(Vector2Int containerPosition, Vector2Int contentPosition)> possibleObjects = new();
            foreach (var objectContainer in _model.LevelObjectContrainers.ObjectsContainers)
            {
                for (int j = 0; j < objectContainer.Columns.Length; j++)
                {
                    for (int k = 0; k < objectContainer.Columns[j].Rows.Length; k++)
                    {
                        Row row = objectContainer.Columns[j].Rows[k];
                        if (row.Type > 0 && row.Type != ObjectType.RedBall && row.Type != ObjectType.GreenBall && row.Type != ObjectType.BlueBall)
                        {
                            possibleObjects.Add((objectContainer.ContainerPosition, new Vector2Int(j, k)));
                        }
                    }
                }
            }

            if (possibleObjects.Count > 0)
            {
                List<(Vector2Int containerPosition, Vector2Int contentPosition)> selectedObjects = new List<(Vector2Int containerPosition, Vector2Int contentPosition)>();
                int maxIndex = possibleObjects.Count - 1;
                for (int i = 0; i < amount; i++)
                {
                    int randomIndex = Random.Range(0, maxIndex + 1);
                    selectedObjects.Add(possibleObjects[randomIndex]);
                    possibleObjects.RemoveAt(randomIndex);
                    maxIndex--;
                }

                foreach (var selectedObject in selectedObjects)
                {
                    _gridController.HideSubgridObject(selectedObject.containerPosition, selectedObject.contentPosition);
                    ObjectContainer objectContainer = _model.LevelObjectContrainers.ObjectsContainers.First(x => x.ContainerPosition == selectedObject.containerPosition);
                    objectContainer.Columns[selectedObject.contentPosition.x].Rows[selectedObject.contentPosition.y].IsHided = true;
                }
            }
        }

        private void AddObjectsToPositions(List<ObjectType> objectTypes, List<LevelPositionData> emptyPositions, int maxObjectCount = 3)
        {
            if (emptyPositions.Count >= objectTypes.Count)
            {
                foreach (var objectType in objectTypes)
                {
                    bool objectPlaced = false;
                    ShufflePositions(emptyPositions);
                    for (int attempt = 0; attempt < 1000; attempt++)
                    {
                        var position = emptyPositions[0];
                        int contentCount = GetContentCountIntoContainer(position.ContainerPosition, objectType, 0);

                        if (contentCount < maxObjectCount)
                        {
                            AddObject(position.ContainerPosition, position.ObjectPosition, objectType, false, false, false);
                            emptyPositions.RemoveAt(0);
                            objectPlaced = true;
                            break;
                        }
                        emptyPositions.RemoveAt(0);
                    }

                    if (!objectPlaced)
                    {
                        Debug.LogWarning("Cannot place object due to lack of available positions. Please adjust the config.");
                    }
                }
            }
        }

        private void AddObject(Vector2Int gridPosition, Vector2Int subgridPosition, ObjectType objectType, bool isLocked, bool isHided, bool isPacked)
        {
            bool isPreviouslyLocked = _model.FindObjectByPosition(gridPosition, subgridPosition).IsLocked;
            _model.SetContainerContent(gridPosition, subgridPosition, objectType, isPreviouslyLocked ? true : isLocked, isHided, isPacked);
            Sprite sprite = _containerObjectDatabase.ContainerObjects.FirstOrDefault(container => container.Type == objectType)?.Sprite;
            _gridController.AddSubgridObject(gridPosition, subgridPosition, sprite, isLocked, objectType == ObjectType.Bird ? true : false);
            if (isHided)
            {
                _gridController.HideSubgridObject(gridPosition, subgridPosition);
            }
            if (isPacked)
            {
                _gridController.PackSubgridObject(gridPosition, subgridPosition);
            }
        }

        private void ShufflePositions(List<LevelPositionData> positions)
        {
            var n = positions.Count;

            for (int i = 0; i < n; i++)
            {
                var r = i + Random.Range(0, n - i);
                (positions[i], positions[r]) = (positions[r], positions[i]);
            }
        }

        public int GetContentCountIntoContainer(Vector2Int containerPosition, ObjectType type, int row)
        {
            ObjectContainer matchingContainer = _model.FindContainerByPosition(containerPosition);
            int count = 0;
            foreach (var column in matchingContainer.Columns)
            {
                if (row >= column.Rows.Length)
                {
                    continue;
                }

                if (column.Rows[row].Type == type)
                {
                    count++;
                }
            }

            return count;
        }

        public float GetLevelTime()
        {
            return _levelDatabase.Levels[_selectedLevel].LevelTime;
        }

        public ObjectType CheckContainerContent(Vector2Int containerPosition, Vector2Int contentPosition)
        {
            return _model.FindObjectByPosition(containerPosition, contentPosition).Type;
        }

        public void MoveContainerObject(Vector2Int containerPosition, Vector2Int objectPosition, Vector2Int newContainerPosition, Vector2Int newobjectPosition)
        {
            Row row = _model.FindObjectByPosition(containerPosition, objectPosition);
            ObjectType newType = row.Type;
            if (row.Type == ObjectType.RedBall)
            {
                newType = ObjectType.GreenBall;
            }
            else if (row.Type == ObjectType.GreenBall)
            {
                newType = ObjectType.BlueBall;
            }
            else if (row.Type == ObjectType.BlueBall)
            {
                newType = ObjectType.RedBall;
            }

            _gridController.ChangeSubgridObjectSprite(newContainerPosition, newobjectPosition, _containerObjectDatabase.ContainerObjects.First(x => x.Type == newType).Sprite);

            _model.SetContainerContent(newContainerPosition, newobjectPosition, newType, false, row.IsHided, row.IsPacked);
            _model.SetContainerContent(containerPosition, objectPosition, ObjectType.None, false, false, false);
            CheckBackgroundItems(containerPosition);
        }

        public bool CheckIsObjectLocked(Vector2Int containerPosition, Vector2Int objectPosition, bool isTargetObject)
        {
            bool result = false;
            ObjectContainer objectContainer = _model.FindContainerByPosition(containerPosition);
            if (objectContainer.IsLocked)
            {
                result = true;
            }
            for (int i = 0; i < objectContainer.Columns[objectPosition.x].Rows.Length; i++)
            {
                if (objectContainer.Columns[objectPosition.x].Rows[i].IsLocked)
                {
                    result = true;
                }
            }

            if (_isTutorialActive)
            {
                if (isTargetObject)
                {
                    if (_tutorialService.IsPlaceRestricted(containerPosition, objectPosition))
                    {
                        result = true;
                    }
                }
                else
                {
                    if (_tutorialService.IsDragRestricted(containerPosition, objectPosition))
                    {
                        result = true;
                    }
                }

                _tutorialService.DragConditionCheck(containerPosition, objectPosition);
            }

            return result;
        }

        public void CheckObjectsInFirstRow(Vector2Int containerPosition)
        {
            ObjectContainer matchingContainer = _model.FindContainerByPosition(containerPosition);

            if (matchingContainer.Columns[0].Rows[0].IsPacked && matchingContainer.ContainerType == ContainerType.Container)
            {
                CheckPackedRow(matchingContainer);
            }
            else
            {
                var firstValue = matchingContainer.Columns[0].Rows[0].Type;
                if (_megaObjectTypes.Any(x => x == firstValue))
                {
                    if (firstValue > 0 && matchingContainer.ContainerType != ContainerType.Container)
                    {
                        CheckRow(matchingContainer, firstValue);
                    }
                }
                else
                {
                    if (firstValue > 0 && matchingContainer.ContainerType == ContainerType.Container)
                    {
                        CheckRow(matchingContainer, firstValue);
                    }
                }
            }
        }

        private void CheckRow(ObjectContainer matchingContainer, ObjectType firstValue)
        {
            int rowSize = matchingContainer.Columns.Length - 1;

            for (int i = 1; i < matchingContainer.Columns.Length; i++)
            {
                if (matchingContainer.Columns[i].Rows[0].Type == firstValue && !matchingContainer.Columns[i].Rows[0].IsPacked)
                {
                    rowSize--;
                    if (rowSize == 0)
                    {
                        if(firstValue == ObjectType.Bird)
                        {
                            StaticPrefs.IsDodoCombinationCompleted = true;
                            _mainUIPanel.AddDodoBird(_gridController.GetGridObject(matchingContainer.ContainerPosition).transform.position);
                            AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameBird);
                        }
                        AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameRowCollapse);
                        RemoveRow(matchingContainer, firstValue);
                        break;
                    }
                }
            }
        }

        private void CheckPackedRow(ObjectContainer matchingContainer)
        {
            int rowSize = matchingContainer.Columns.Length - 1;

            for (int i = 1; i < matchingContainer.Columns.Length; i++)
            {
                if (matchingContainer.Columns[i].Rows[0].IsPacked)
                {
                    rowSize--;
                    if (rowSize == 0)
                    {
                        UnpackRow(matchingContainer);
                        break;
                    }
                }
            }
        }

        private void UnpackRow(ObjectContainer objectContainer)
        {
            for (int i = 0; i < objectContainer.Columns.Length; i++)
            {
                Column column = objectContainer.Columns[i];
                column.Rows[0].IsPacked = false;
                _gridController.UnpackSubgridObject(objectContainer.ContainerPosition, new Vector2Int(i, 0));
            }
        }

        public bool ShuffleObjects()
        {
            List<LevelPositionData> emptyPositions = _model.GetPositionsData(true, _model.IsBackLineActive);
            List<LevelPositionData> occupiedPositions = _model.GetPositionsData(false, _model.IsBackLineActive);

            if (emptyPositions.Count > 0)
            {
                for (int i = occupiedPositions.Count - 1; i >= 0; i--)
                {
                    int randomIndex = Random.Range(0, emptyPositions.Count);
                    LevelPositionData emptyPosition = emptyPositions[randomIndex];

                    _gridController.MoveSubgridObject(occupiedPositions[i].ContainerPosition,
                        occupiedPositions[i].ObjectPosition, emptyPosition.ContainerPosition, emptyPosition.ObjectPosition);

                    Row row = _model.FindObjectByPosition(occupiedPositions[i].ContainerPosition, occupiedPositions[i].ObjectPosition);
                    _model.SetContainerContent(occupiedPositions[i].ContainerPosition, occupiedPositions[i].ObjectPosition, ObjectType.None, false, false, false);
                    _model.SetContainerContent(emptyPosition.ContainerPosition, emptyPosition.ObjectPosition, row.Type, false, row.IsHided, row.IsPacked);

                    emptyPositions.Add(new LevelPositionData(occupiedPositions[i].ContainerPosition, occupiedPositions[i].ObjectPosition));
                    emptyPositions.RemoveAt(randomIndex);
                }

                foreach (var container in _model.LevelObjectContrainers.ObjectsContainers)
                {
                    CheckObjectsInFirstRow(container.ContainerPosition);
                }

                CheckAllBackgroundItems();

                if (_isTutorialActive)
                {
                    _tutorialService.TutorialButtonCheck(1);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveClueObjects()
        {
            Debug.Log("Removing Clue Objects!!!");
            List<(Vector2Int gridPosition, Vector2Int subgridPosition, ObjectType value)> occupiedPositions = new();

            for (int i = 0; i < _model.LevelObjectContrainers.ObjectsContainers.Length; i++)
            {
                ObjectContainer container = _model.LevelObjectContrainers.ObjectsContainers[i];
                for (int j = 0; j < container.Columns.Length; j++)
                {
                    for (int k = 0; k < container.Columns[j].Rows.Length; k++)
                    {
                        bool isMegaObjectType = _megaObjectTypes.Any(x => x == container.Columns[j].Rows[k].Type);
                        bool isPacked = container.Columns[j].Rows[k].IsPacked;
                        if (container.Columns[j].Rows[k].Type > 0 && !isMegaObjectType && !isPacked)
                        {
                            occupiedPositions.Add((_model.LevelObjectContrainers.ObjectsContainers[i].ContainerPosition,
                                new Vector2Int(j, k), container.Columns[j].Rows[k].Type));
                        }
                    }
                }
            }

            if (occupiedPositions.Count > 0)
            {
                var duplicateArrays = occupiedPositions
                    .GroupBy(x => x.value)
                    .Where(g => g.Count() >= 3)
                    .Select(g => g.Take(3).ToArray())
                    .ToList();

                if (duplicateArrays.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, duplicateArrays.Count);

                    var randomArray = duplicateArrays[randomIndex];

                    foreach (var item in randomArray)
                    {
                        _gridController.DestroySubgridObject(item.gridPosition, item.subgridPosition, false);
                        _model.SetContainerContent(item.gridPosition, item.subgridPosition, ObjectType.None, false, false, false);
                    }
                    CheckEndGame();
                    CheckAllBackgroundItems();
                    AddCompletedCombinations();
                    if (randomArray[0].value == ObjectType.Bird)
                    {
                        StaticPrefs.IsDodoCombinationCompleted = true;
                        _mainUIPanel.AddDodoBird(_gridController.GetGridObject(randomArray[0].gridPosition).transform.position);
                        AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameBird);
                    }
                    AudioManager.Instance.PlayAudioEvent(AudioEventType.CoreGameRowCollapse);

                    if (_isTutorialActive)
                    {
                        _tutorialService.TutorialButtonCheck(0);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool CheckHungryPersonObject(Vector2Int containerPosition, Vector2Int contentPosition)
        {
            if (_hungryPersonObjects.Count > 0)
            {
                Row row = _model.FindObjectByPosition(containerPosition, contentPosition);
                if(row.Type == _hungryPersonObjects[0] && !row.IsPacked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void RemovePersonObject(Vector2Int containerPosition, Vector2Int contentPosition)
        {
            if(_isTutorialActive)
            {
                _tutorialService.PersonConditionCheck();
            }
            _model.SetContainerContent(containerPosition, contentPosition, ObjectType.None, false, false, false);
            _hungryPersonObjects.RemoveAt(0);
            if (_hungryPersonObjects.Count > 0)
            {
                _hungryPerson.SetRequiredObjectSprite(_containerObjectDatabase.ContainerObjects.First(x => x.Type == _hungryPersonObjects[0]).Sprite);
                _hungryPerson.SetEating();
            }
            else
            {
                _hungryPerson.SetDisappearing();
            }
            CheckEndGame();
            CheckAllBackgroundItems();
        }

        public Vector3 GetHungryPersonPosition()
        {
            return _hungryPerson.gameObject.transform.position;
        }

        private void RemoveRow(ObjectContainer objectContainer, ObjectType objectType)
        {
            for (int i = 0; i < objectContainer.Columns.Length; i++)
            {
                _gridController.DestroySubgridObject(objectContainer.ContainerPosition, new Vector2Int(i, 0), objectType == ObjectType.Bird);
                _model.SetContainerContent(objectContainer.ContainerPosition, new Vector2Int(i, 0), ObjectType.None, false, false, false);
            }

            CheckBackgroundItems(objectContainer.ContainerPosition);
            CheckEndGame();
            AddCompletedCombinations();
        }

        private void CheckEndGame()
        {
            if (_model.CheckAllRowsAreEmpty())
            {
                CoreGameWonEvent?.Invoke(true, StaticPrefs.LastStartedCoreGameLevel);
                _currencyStorage.Add(_levelDatabase.Levels[_selectedLevel].LevelReward);
                GameWonEvent?.Invoke(_levelDatabase.Levels[_selectedLevel].LevelReward, _selectedLevel, _levelDatabase.Levels[_selectedLevel].IsNextLevelButtonDeactivated);
                AudioManager.Instance?.PlayAudioEvent(AudioEventType.WinCoreGame);
            }
        }

        private void AddCompletedCombinations()
        {
            _completedCombinations++;
            if (_levelDatabase.Levels[_selectedLevel].LockedContainers.Count > 0)
            {
                foreach (var lockedContainer in _levelDatabase.Levels[_selectedLevel].LockedContainers)
                {
                    if (_completedCombinations >= lockedContainer.UnlockCombinations)
                    {
                        _model.LevelObjectContrainers.ObjectsContainers[lockedContainer.ID].IsLocked = false;
                        _gridController.GetGridObject(_model.LevelObjectContrainers.ObjectsContainers[lockedContainer.ID].ContainerPosition).SetLock(false, 0);
                    }
                    else
                    {
                        _gridController.GetGridObject(_model.LevelObjectContrainers.ObjectsContainers[lockedContainer.ID].ContainerPosition)
                            .SetLock(true, lockedContainer.UnlockCombinations - _completedCombinations);
                    }
                }
            }
        }

        private void CheckBackgroundItems(Vector2Int containerPosition)
        {
            bool isFirstRowEmpty = true;
            ObjectContainer matchingContainer = _model.FindContainerByPosition(containerPosition);

            foreach (var column in matchingContainer.Columns)
            {
                if (column.Rows[0].Type > 0)
                {
                    isFirstRowEmpty = false;
                    break;
                }
            }

            if (isFirstRowEmpty)
            {
                for (int i = 0; i < matchingContainer.Columns.Length; i++)
                {
                    Column column = matchingContainer.Columns[i];
                    if (column.Rows[1].Type > 0 && column.Rows[0].Type == 0)
                    {
                        Row row = _model.FindObjectByPosition(containerPosition, new Vector2Int(i, 1));

                        _model.SetContainerContent(
                            containerPosition,
                            new Vector2Int(i, 0),
                            row.Type,
                            false,
                            row.IsHided,
                            row.IsPacked
                            );
                        _model.SetContainerContent(containerPosition, new Vector2Int(i, 1), ObjectType.None, false, false, false);
                        _gridController.MoveSubgridObject(containerPosition, new Vector2Int(i, 1), containerPosition, new Vector2Int(i, 0));
                    }
                }
            }
        }

        private void CheckAllBackgroundItems()
        {
            foreach (var objectContainer in _model.LevelObjectContrainers.ObjectsContainers)
            {
                CheckBackgroundItems(objectContainer.ContainerPosition);
            }
        }
    }
}
