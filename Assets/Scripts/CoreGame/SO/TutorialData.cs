using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreGame.SO
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "SO/CoreGame/TutorialData")]
    public class TutorialData : ScriptableObject
    {
        public List<TutorialTask> Tasks;

#if UNITY_EDITOR
        public List<KeyValuePair<int, TutorialTaskType>> PreviousTasksTypes;
        [HideInInspector] public TutorialTaskType[] PlayModeTaskTypes;

        public void OnValidate()
        {
            Undo.RecordObject(this, "");
            if (PlayModeTaskTypes == null || PlayModeTaskTypes.Length != Tasks.Count)
            {
                PlayModeTaskTypes = new TutorialTaskType[Tasks.Count];
            }

            if (PreviousTasksTypes == null)
            {
                PreviousTasksTypes = new List<KeyValuePair<int, TutorialTaskType>>();
            }

            List<KeyValuePair<int, TutorialTaskType>> previousList = new List<KeyValuePair<int, TutorialTaskType>>(PreviousTasksTypes);

            PreviousTasksTypes.Clear();

            for (int i = 0; i < Tasks.Count; i++)
            {
                TutorialTask task = Tasks[i];
                TutorialTaskType previousType = TutorialTaskType.None;

                if (i < previousList.Count && previousList[i].Key == i)
                {
                    previousType = previousList[i].Value;
                }

                if( previousType == TutorialTaskType.None)
                {
                    previousType = PlayModeTaskTypes[i];
                }

                if (previousType != task.Type)
                {
                    switch (task.Type)
                    {
                        case TutorialTaskType.Greetings:
                            task.Task = new GreetingsTask();
                            break;

                        case TutorialTaskType.ButtonGreetings:
                            task.Task = new GreetingsTask();
                            break;

                        case TutorialTaskType.DragToContainer:
                            task.Task = new DragToContainerTask();
                            break;

                        case TutorialTaskType.DragToPerson:
                            task.Task = new DragToPersonTask();
                            break;

                        case TutorialTaskType.Button:
                            task.Task = new ButtonTask();
                            break;
                    }
                }

                PlayModeTaskTypes[i] = task.Type;

                PreviousTasksTypes.Add(new KeyValuePair<int, TutorialTaskType>(i, task.Type));
            }
            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public class TutorialTask
    {
        public TutorialTaskType Type;
        [SerializeReference]
        public ITask Task;
    }

    public interface ITask
    {

    }

    [Serializable]
    public class GreetingsTask : ITask
    {
        public HelpText Help;
    }

    [Serializable]
    public class DragToContainerTask : ITask
    {
        public Vector2Int StartContainerPosition;
        public Vector2Int StartObjectPosition;

        public Vector2Int EndContainerPosition;
        public Vector2Int EndObjectPosition;

        public List<HelpText> Help;
    }

    [Serializable]
    public class DragToPersonTask : ITask
    {
        public Vector2Int StartContainerPosition;
        public Vector2Int StartObjectPosition;

        public List<HelpText> Help;
    }

    [Serializable]
    public class ButtonTask : ITask
    {
        public int ButtonId = -1;

        public List<HelpText> Help;
    }

    [Serializable]
    public class HelpText
    {
        public HelpState State;
        public BubbleSize BubbleSize;
        [TextArea] public string Text;
    }

    public enum TutorialTaskType
    {
        None,
        DragToContainer,
        DragToPerson,
        Button,
        Greetings,
        ButtonGreetings
    }

    public enum HelpState
    {
        None,
        LeftButton,
        RightButton,
        FirstPerson,
        SecondPerson,
        FullScreenPerson
    }

    public enum BubbleSize
    {
        None,
        Small,
        Medium,
        Large
    }
}
