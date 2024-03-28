using UnityEngine;
using Web;

namespace VillageGame.Logic.Debuging
{
    public class GuiLog : MonoBehaviour
    {
        private const float UpdateInterval = 1.0f;
        private float _timeleft;
        private float _lastTime;
        private float _timeSpan;
        private int _lastFrame;
        private int _frames;
        private float _fps;
        [SerializeField] private string _buildNumber;

        /*
        private string webState = "Close";
        private void OnEnable()
        {
            WebSocketClient.ConnectedEvent += ConnectedEvent;
        }

        private void OnDisable()
        {
            WebSocketClient.ConnectedEvent += ConnectedEvent;
        }

        private void ConnectedEvent()
        {
            webState = "open";
        }

        void Update()
        {
            _timeleft += Time.deltaTime;
            if (_timeleft > UpdateInterval)
            {
                _timeleft -= UpdateInterval;
                _frames = Time.frameCount - _lastFrame;
                _lastFrame = Time.frameCount;
                _timeSpan = Time.realtimeSinceStartup - _lastTime;
                _lastTime = Time.realtimeSinceStartup;
                _fps = Mathf.RoundToInt(_frames / _timeSpan);
            }
        }
        */

        void OnGUI()
        {
            /*GUI.Box(new Rect(10, 10, 70, 25), string.Format("fps {0}", _fps));
            GUI.Box(new Rect(90, 10, 150, 25), string.Format("state: {0}", webState));*/
            GUI.Box(new Rect(Screen.width /2f, Screen.height - 45,100, 25), $"{_buildNumber}",
                new GUIStyle {fontSize = 20});
        }
    }
}