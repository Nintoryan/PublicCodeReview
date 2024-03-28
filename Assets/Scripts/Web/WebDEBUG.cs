using UnityEngine;
using Util;

namespace Web
{
    public class WebDEBUG : MonoBehaviour
    {
        public static void Log(string msg)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
#pragma warning disable CS0618
                Application.ExternalCall("log", msg);
#pragma warning restore CS0618
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
            }

            Debugging.Log(msg, ColorType.Orange);
        }
    }
}