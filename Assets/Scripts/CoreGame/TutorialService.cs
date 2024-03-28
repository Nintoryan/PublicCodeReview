using CoreGame.SO;
using System.Collections.Generic;
using Tutorial;
using UnityEngine;
using VillageGame.Logic.Tutorial;
using VillageGame.UI.Buttons;

namespace CoreGame
{
    public class TutorialService : MonoBehaviour
    {
        [SerializeField] private TutorialFinger _tutorialFinger;
        [SerializeField] private TutorialBlackScreen _tutorialBlackScreen;
        [SerializeField] private TutorialCanvas _tutorialCanvas;
        [SerializeField] private LevelStorage _levelStorage;
        [SerializeField] private GridController _gridController;
        [SerializeField] private List<UIButton> _tutorialButtons;
        [SerializeField] private TalkingPerson _talkingPerson;

        private TutorialData _tutorialData;
        private int _currentTaskId = -1;
        private Transform _initialButtonParent;
        private bool _isClickAwait;

        private void Update()
        {
            if (_isClickAwait)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isClickAwait = false;
                    GreetingsCheck();
                }
            }
        }

        public void StartTutorial(TutorialData tutorialData)
        {
            _tutorialData = tutorialData;
            StartTask();
        }

        public bool IsDragRestricted(Vector2Int containerPosition, Vector2Int objectPosition)
        {
            bool result = false;

            if (_currentTaskId >= 0)
            {
                switch (_tutorialData.Tasks[_currentTaskId].Type)
                {
                    case TutorialTaskType.DragToContainer:
                        DragToContainerTask dragToContainerTask = (DragToContainerTask)_tutorialData.Tasks[_currentTaskId].Task;

                        if (dragToContainerTask.StartContainerPosition != containerPosition || dragToContainerTask.StartObjectPosition != objectPosition)
                        {
                            result = true;
                        }
                        break;

                    case TutorialTaskType.DragToPerson:
                        DragToPersonTask dragToPersonTask = (DragToPersonTask)_tutorialData.Tasks[_currentTaskId].Task;

                        if (dragToPersonTask.StartContainerPosition != containerPosition || dragToPersonTask.StartObjectPosition != objectPosition)
                        {
                            result = true;
                        }
                        break;

                    case TutorialTaskType.Button:
                        if (_currentTaskId >= 0)
                        {
                            result = true;
                        }
                        break;

                    default:
                        Debug.LogWarning("Incorrect type");
                        break;

                }
            }

            return result;
        }

        public bool IsPlaceRestricted(Vector2Int containerPosition, Vector2Int objectPosition)
        {
            bool result = false;
            if (_currentTaskId >= 0)
            {
                if (_tutorialData.Tasks[_currentTaskId].Type == TutorialTaskType.DragToContainer)
                {
                    DragToContainerTask dragToContainerTask = (DragToContainerTask)_tutorialData.Tasks[_currentTaskId].Task;

                    if (dragToContainerTask.EndContainerPosition != containerPosition || dragToContainerTask.EndObjectPosition != objectPosition)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public void DragConditionCheck(Vector2Int containerPosition, Vector2Int objectPosition)
        {
            if (_currentTaskId >= 0)
            {
                if (_tutorialData.Tasks[_currentTaskId].Type == TutorialTaskType.DragToContainer)
                {
                    DragToContainerTask dragToContainerTask = (DragToContainerTask)_tutorialData.Tasks[_currentTaskId].Task;

                    if (dragToContainerTask.EndContainerPosition == containerPosition && dragToContainerTask.EndObjectPosition == objectPosition)
                    {
                        if (_currentTaskId != _tutorialData.Tasks.Count - 1)
                        {
                            StartTask();
                        }
                        else
                        {
                            EndTask();
                        }
                    }
                }
            }
        }

        public void PersonConditionCheck()
        {
            if (_currentTaskId >= 0)
            {
                if (_currentTaskId != _tutorialData.Tasks.Count - 1)
                {
                    StartTask();
                }
                else
                {
                    EndTask();
                }
            }
        }

        public void TutorialButtonCheck(int buttonId)
        {
            if (_currentTaskId >= 0)
            {
                if (_tutorialData.Tasks[_currentTaskId].Type == TutorialTaskType.Button)
                {
                    ButtonTask buttonTask = (ButtonTask)_tutorialData.Tasks[_currentTaskId].Task;

                    if (buttonTask.ButtonId == buttonId)
                    {
                        if (_currentTaskId != _tutorialData.Tasks.Count - 1)
                        {
                            StartTask();
                        }
                        else
                        {
                            EndTask();
                        }
                    }
                }
            }
        }

        public void GreetingsCheck()
        {
            if (_currentTaskId >= 0)
            {
                if (_tutorialData.Tasks[_currentTaskId].Type == TutorialTaskType.Greetings)
                {
                    if (_currentTaskId != _tutorialData.Tasks.Count - 1)
                    {
                        StartTask();
                    }
                    else
                    {
                        EndTask();
                    }
                }
            }
        }

        public void ButtonGreetingsCheck()
        {
            if (_currentTaskId >= 0)
            {
                if (_tutorialData.Tasks[_currentTaskId].Type == TutorialTaskType.ButtonGreetings)
                {
                    if (_currentTaskId != _tutorialData.Tasks.Count - 1)
                    {
                        StartTask();
                    }
                    else
                    {
                        EndTask();
                    }
                }
            }
        }

        private void StartTask()
        {
            _currentTaskId++;
            switch (_tutorialData.Tasks[_currentTaskId].Type)
            {
                case TutorialTaskType.ButtonGreetings:
                case TutorialTaskType.Greetings:
                    _tutorialBlackScreen.SetActive(true);
                    GreetingsTask greetingsTask = (GreetingsTask)_tutorialData.Tasks[_currentTaskId].Task;

                    _talkingPerson.ActivatePerson(greetingsTask.Help.State, greetingsTask.Help.BubbleSize, greetingsTask.Help.Text);
                    _isClickAwait = true;
                    break;

                case TutorialTaskType.DragToContainer:
                    DragToContainerTask dragToContainerTask = (DragToContainerTask)_tutorialData.Tasks[_currentTaskId].Task;

                    _tutorialFinger.ShowAndMoveBetweenWordsPoints(
                        _gridController.GetObjectPosition(dragToContainerTask.StartContainerPosition, dragToContainerTask.StartObjectPosition),
                        _gridController.GetObjectPosition(dragToContainerTask.EndContainerPosition, dragToContainerTask.EndObjectPosition),
                        Vector3.zero
                        );
                    _talkingPerson.DeactivatePerson();
                    _tutorialBlackScreen.SetActive(false);
                    foreach (var button in _tutorialButtons)
                    {
                        button.DisableButton();
                    }

                    if (dragToContainerTask.Help.Count > 0)
                    {
                        CheckHelp(dragToContainerTask.Help[0]);
                    }
                    break;

                case TutorialTaskType.DragToPerson:
                    DragToPersonTask dragToPersonTask = (DragToPersonTask)_tutorialData.Tasks[_currentTaskId].Task;

                    _tutorialFinger.ShowAndMoveBetweenWordsPoints(
                        _gridController.GetObjectPosition(dragToPersonTask.StartContainerPosition, dragToPersonTask.StartObjectPosition),
                        _levelStorage.GetHungryPersonPosition(),
                        Vector3.zero,
                        1.5f
                        );
                    _talkingPerson.DeactivatePerson();
                    _tutorialBlackScreen.SetActive(false);
                    foreach (var button in _tutorialButtons)
                    {
                        button.DisableButton();
                    }

                    if (dragToPersonTask.Help.Count > 0)
                    {
                        CheckHelp(dragToPersonTask.Help[0]);
                    }
                    break;

                case TutorialTaskType.Button:
                    ButtonTask buttonTask = (ButtonTask)_tutorialData.Tasks[_currentTaskId].Task;

                    TutorialFinger.AngleType angleType;
                    Vector3 offset;

                    switch (buttonTask.ButtonId)
                    {
                        case 0:
                            angleType = TutorialFinger.AngleType.DownRight;
                            offset = new Vector3(-1.8f, 2.3f, 0);
                            break;
                        case 1:
                            angleType = TutorialFinger.AngleType.DownLeft;
                            offset = new Vector3(2.3f, 1.8f, 0);
                            break;
                        default:
                            angleType = TutorialFinger.AngleType.UpRight;
                            offset = Vector3.zero;
                            Debug.LogWarning("Incorrect type");
                            break;
                    }
                    _tutorialFinger.PlayBounceInWordPoint(_tutorialButtons[buttonTask.ButtonId].body.transform.position, offset, angleType);
                    _talkingPerson.DeactivatePerson();
                    _tutorialBlackScreen.SetActive(false);
                    foreach (var button in _tutorialButtons)
                    {
                        button.DisableButton();
                    }

                    _initialButtonParent = _tutorialButtons[buttonTask.ButtonId].gameObject.transform.parent;
                    _tutorialButtons[buttonTask.ButtonId].EnableButton();
                    _tutorialButtons[buttonTask.ButtonId].gameObject.transform.SetParent(_tutorialCanvas.Body.transform);
                    _tutorialFinger.transform.SetAsLastSibling();

                    if (buttonTask.Help.Count > 0)
                    {
                        CheckHelp(buttonTask.Help[0]);
                    }
                    break;

                default:
                    Debug.LogWarning("Incorrect type");
                    break;

            }
        }

        private void CheckHelp(HelpText helpText)
        {
            _talkingPerson.ActivatePerson(helpText.State, helpText.BubbleSize, helpText.Text);
        }

        private void EndTask()
        {
            _tutorialFinger.StopAnimation();
            _tutorialBlackScreen.SetActive(false);
            switch (_tutorialData.Tasks[_currentTaskId].Type)
            {
                case TutorialTaskType.Greetings:
                    _tutorialBlackScreen.SetActive(false);
                    _talkingPerson.DeactivatePerson();
                    break;

                case TutorialTaskType.DragToContainer:
                    foreach (var button in _tutorialButtons)
                    {
                        button.EnableButton();
                    }
                    _talkingPerson.DeactivatePerson();
                    break;

                case TutorialTaskType.DragToPerson:
                    foreach (var button in _tutorialButtons)
                    {
                        button.EnableButton();
                    }
                    _talkingPerson.DeactivatePerson();
                    break;

                case TutorialTaskType.Button:
                    ButtonTask buttonTask = (ButtonTask)_tutorialData.Tasks[_currentTaskId].Task;

                    foreach (var button in _tutorialButtons)
                    {
                        button.EnableButton();
                    }
                    _talkingPerson.DeactivatePerson();
                    _tutorialButtons[buttonTask.ButtonId].gameObject.transform.SetParent(_initialButtonParent);

                    break;

                default:
                    Debug.LogWarning("Incorrect type");
                    break;
            }

            if (_currentTaskId == _tutorialData.Tasks.Count - 1)
            {
                _currentTaskId = -1;
            }
        }
    }
}
