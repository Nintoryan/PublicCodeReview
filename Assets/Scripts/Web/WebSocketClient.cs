using System.Collections;
using System.Text;
using NativeWebSocket;
using UnityEngine;
using UnityEngine.Events;
using Util;
using Web.Api;

namespace Web
{
    public class WebSocketClient : MonoBehaviour
    {
        [SerializeField] private bool _isGameWithWebSocket;
        private static WebSocket _websocket;
        public static event UnityAction<string> ServerMessage;

        private static WebSocketClient _instance;
        public static UnityAction ConnectedEvent;
        public static bool IsConnectedOnce { get; private set; }
        private float _reconnectTimer;
        private int _currentReconnectAwait;

        private void Awake()
        {
            _instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => JSAPI.UID != "");
            
            if (!_isGameWithWebSocket)
            {
                ConnectedEvent?.Invoke();
                yield break;
            }
            _websocket = new WebSocket($"wss://village.dodopizza.com/api/ws/?uid={JSAPI.UID}");
            
            _websocket.OnOpen += OnOpen;
            _websocket.OnError += OnError;
            _websocket.OnClose += OnClose;
            _websocket.OnMessage += OnMessage;

            WebDEBUG.Log($"Starting websocket client...{JSAPI.UID}");
            var connectTask = _websocket.Connect();
            
            while (!connectTask.IsCompleted)
            {
                yield return null;
            }
            
            WebDEBUG.Log("WebSocket client started.");
            ConnectedEvent?.Invoke();
        }
        
        private void OnOpen()
        {
            WebDEBUG.Log("Connection open!");
            ConnectionErrorHandler.Hide();
            ConnectedEvent?.Invoke();
            IsConnectedOnce = true;
            _currentReconnectAwait = 1;
        }

        private void OnError(string errormsg)
        {
            WebDEBUG.Log("Error! " + errormsg);
        }

        private void OnMessage(byte[] data)
        {
            var message = Encoding.UTF8.GetString(data);
            WebDEBUG.Log($"ServerMESSAGE:{message}");
            ServerMessage?.Invoke(message);
        }

        private void OnClose(WebSocketCloseCode closecode)
        {     
            WebDEBUG.Log("Connection closed!");
            _reconnectTimer = _currentReconnectAwait;
            ConnectionErrorHandler.Show();
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (!_isGameWithWebSocket || _websocket == null) return;

            _websocket.DispatchMessageQueue();
#endif
            if(_websocket == null) return;

            if (_websocket.State == WebSocketState.Open || !IsConnectedOnce)
            {
                ConnectionErrorHandler.Hide();
            }
            else
            {
                ConnectionErrorHandler.Show();
            }

            if (_websocket.State != WebSocketState.Closed) return;
            
            if (_reconnectTimer < 0)
            {
                _currentReconnectAwait++;
                _reconnectTimer = _currentReconnectAwait;
                StartCoroutine(Start());
                Debugging.Log($"Attempt to reconnect ws. Number:{_currentReconnectAwait}");
            }
            else
            {
                _reconnectTimer -= Time.deltaTime;
            }
        }

        public static void SendMessageToServer(string message)
        {
            _instance.SendWebSocketMessage(message);
            
        }

        private async void SendWebSocketMessage(string message)
        {
            if (!_isGameWithWebSocket) return;
            if (_websocket != null && _websocket.State == WebSocketState.Open)
            {
                await _websocket.SendText(message);
            }
        }

        public static void Close()
        {
            _websocket?.Close();
        }
        
        private void OnDisable()
        {
           Close();
        }
    }
}
