using UnityEngine;

namespace Util
{
    public enum ColorType
    {
        Lime,
        Red,
        Orange,
        Green,
        Blue,
        Aqua,
        Lightblue,
        Magenta,
        Brown,
        Olive,
        Teal,
        Purple
    }

    public enum LogStyle
    {
        LogMessage,
        Error,
        Warning
    }

    public static class Debugging
    {
        /// <summary>
        /// Simple custom color Log massage
        /// </summary>
        /// <param name="value"> String message</param>
        /// <param name="color"> Color of message</param>
        /// <param name="style"> Type of message</param>
        public static void Log(string value, ColorType color, LogStyle style = LogStyle.LogMessage)
        {
            switch (style)
            {
                case LogStyle.LogMessage:
                    Debug.Log($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Error:
                    Debug.LogError($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Warning:
                    Debug.LogWarning($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
            }
        }

        /// <summary>
        /// Simple color Log massage
        /// </summary>
        /// <param name="value"> String message</param>
        /// <param name="style"> Type of message</param>
        public static void Log(string value, LogStyle style = LogStyle.LogMessage)
        {
            switch (style)
            {
                case LogStyle.LogMessage:
                    Debug.Log(
                        $"<color={ColorType.Lime.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Error:
                    Debug.LogError(
                        $"<color={ColorType.Red.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Warning:
                    Debug.LogWarning(
                        $"<color={ColorType.Orange.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
            }
        }

        /// <summary>
        /// Simple color Log massage in Editor ONLY
        /// </summary>
        /// <param name="value"> String message</param>
        /// <param name="style"> Type of message</param>
        public static void ColorLogEditorOnly(string value, LogStyle style = LogStyle.LogMessage)
        {
#if UNITY_EDITOR
            switch (style)
            {
                case LogStyle.LogMessage:
                    Debug.Log(
                        $"<color={ColorType.Lime.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Error:
                    Debug.LogError(
                        $"<color={ColorType.Red.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Warning:
                    Debug.LogWarning(
                        $"<color={ColorType.Orange.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
            }     
#endif
        }

        /// <summary>
        /// Simple color Log massage in Editor ONLY
        /// </summary>
        /// <param name="value"> String message</param>
        /// <param name="color"> Color of message</param>
        /// <param name="style"> Type of message</param>
        public static void ColorLogEditorOnly(string value, ColorType color, LogStyle style = LogStyle.LogMessage)
        {
#if UNITY_EDITOR
            switch (style)
            {
                case LogStyle.LogMessage:
                    Debug.Log($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Error:
                    Debug.LogError($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
                case LogStyle.Warning:
                    Debug.LogWarning($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>: {value}");
                    break;
            }      
#endif
        }

        /// <summary>
        /// Multiple new line Log color message.
        ///  Color by style log
        /// </summary>
        /// <param name="style"> Type of Log (LogMessage, Error, Warning)</param>
        /// <param name="list"> First object - string message, second - value object...ect </param>
        /// <param name="isOnlyEditor"> Showing only in Editor</param>
        public static void ColorLogParams(LogStyle style, params object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                switch (style)
                {
                    case LogStyle.LogMessage:
                        Debug.Log($"<color={ColorType.Lime.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                  $" {list[i]} is <color={ColorType.Lime.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Error:
                        Debug.LogError(
                            $"<color={ColorType.Red.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                            $" {list[i]} is is <color={ColorType.Red.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Warning:
                        Debug.LogWarning(
                            $"<color={ColorType.Orange.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                            $" {list[i]} is <color={ColorType.Orange.ToString()}>{list[i + 1]}</color>");
                        break;
                }

                i++;
            }
        }


        /// <summary>
        /// Multiple new line Log color message
        /// </summary>
        /// <param name="style"> Type of Log (LogMessage, Error, Warning)</param>
        /// <param name="color"> Color for LogStyle and value object</param>
        /// <param name="list"> First object - string message, second - value object...ect </param>
        public static void ColorLogParams(LogStyle style, ColorType color, params object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                switch (style)
                {
                    case LogStyle.LogMessage:
                        Debug.Log($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                  $" {list[i]} is <color={color.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Error:
                        Debug.LogError($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                       $" {list[i]} is is <color={color.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Warning:
                        Debug.LogWarning($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                         $" {list[i]} is <color={color.ToString()}>{list[i + 1]}</color>");
                        break;
                }

                i++;
            }
        }

        /// <summary>
        /// Multiple new line Log color message Editor ONLY.
        ///  Color by style log
        /// </summary>
        /// <param name="style"> Type of Log (LogMessage, Error, Warning)</param>
        /// <param name="list"> First object - string message, second - value object...ect </param>
        public static void ColorLogParamsEditorOnly(LogStyle style, params object[] list)
        {
#if UNITY_EDITOR
            for (int i = 0; i < list.Length; i++)
            {
                switch (style)
                {
                    case LogStyle.LogMessage:
                        Debug.Log($"<color={ColorType.Lime.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                  $" {list[i]} is <color={ColorType.Lime.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Error:
                        Debug.LogError(
                            $"<color={ColorType.Red.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                            $" {list[i]} is is <color={ColorType.Red.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Warning:
                        Debug.LogWarning(
                            $"<color={ColorType.Orange.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                            $" {list[i]} is <color={ColorType.Orange.ToString()}>{list[i + 1]}</color>");
                        break;
                }

                i++;
            }     
#endif
        }

        /// <summary>
        /// Multiple new line Log color message Editor ONLY.
        /// </summary>
        /// <param name="style"> Type of Log (LogMessage, Error, Warning)</param>
        /// <param name="color"> Color for LogStyle and value object</param>
        /// <param name="list"> First object - string message, second - value object...ect </param>
        public static void ColorLogParamsEditorOnly(LogStyle style, ColorType color, params object[] list)
        {
#if UNITY_EDITOR
            for (int i = 0; i < list.Length; i++)
            {
                switch (style)
                {
                    case LogStyle.LogMessage:
                        Debug.Log($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                  $" {list[i]} is <color={color.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Error:
                        Debug.LogError($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                       $" {list[i]} is is <color={color.ToString()}>{list[i + 1]}</color>");
                        break;
                    case LogStyle.Warning:
                        Debug.LogWarning($"<color={color.ToString()}>==[{style.ToString().ToUpper()}]==</color>:" +
                                         $" {list[i]} is <color={color.ToString()}>{list[i + 1]}</color>");
                        break;
                }

                i++;
            }
#endif
        }
    }
}