using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Web.Api
{
    public class JSAPI: MonoBehaviour
    {
        [SerializeField] private bool _realAPI;

        public static Action Copied;
        public static bool IsAudioOn = true;

        public static string UID { get; private set; } = "";

        public static event UnityAction<string> UIDSet;

        private void Awake()
        {
            if (!_realAPI)
            {
                UID = GetUID().ToString();
                Debug.Log($"uid {UID}");
            }
        }

        public static void SetCustomUID(string uid)
        {
            UID = uid;
        }
        public static void SetTestUserUID()
        {
            UID = "7EAA7548070A8B4611EE3AA4CB0CF8EA";
        }

        [ContextMenu("SetTestUID")]
        public void SetTestUID()
        {
            UID = "7EAA7548070A8B4611EE3AA4CB0CF8EA";
        }
        
        [ContextMenu("SetMyUID")]
        public void SetMyUID()
        {
            UID = "000D3A25D54580E611E75743E72DEA64";
            UIDSet?.Invoke(UID);
        }

        public void SetUID(string json)
        {
            Match match = Regex.Match(json, "\"userId\":\\s*\"([^\"]*)\"");

            if (match.Success)
            {
                UID = match.Groups[1].Value;
                
                UIDSet?.Invoke(UID);

                Debug.Log($"uid {UID}");
            }
            else
            {
                Debug.LogError("Failed to extract userId from JSON");
            }
            
            Debugging.Log($"uid {UID}");        
        }
        
        private long GetUID()
        {
            var deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
            var inputBytes = Encoding.UTF8.GetBytes(deviceUniqueIdentifier);
            
            using SHA256 sha256 = SHA256.Create();
            
            var hashBytes = sha256.ComputeHash(inputBytes);
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "");
            var uid = hashString.Substring(0, 16);
            return long.TryParse(uid, System.Globalization.NumberStyles.HexNumber, null, out var uidValue) ? uidValue : 420;
        }

        public static void ShowCopyToClipboard(string text)
        {
            Application.ExternalCall("showCopyToClipboard",text);
        }

        public static void HideCopyToClipboard()
        {
            Application.ExternalCall("hideCopyToClipboard");
        }

        public static void SendURL(string url)
        {
            Application.ExternalCall("sendUrl",url);
        }
        
        //Вызывается на стороне JS
        public void SetCopied()
        {
            Copied?.Invoke();
        }

        public void LogUnity(string log)
        {
            Debugging.Log($"[JS LOG] {log}");
        }
        
        public void EnableAudio()
        {
            Debugging.Log($"[JS] Enable Audio");
            IsAudioOn = true;
        }
        
        public void DisableAudio()
        {
            Debugging.Log($"[JS] Disable Audio");
            IsAudioOn = false;
        }
    }
}