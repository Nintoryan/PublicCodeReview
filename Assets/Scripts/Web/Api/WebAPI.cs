using System;
using UnityEngine;
using Util;
using VContainer;
using Web.RequestStructs;

namespace Web.Api
{
    public partial class WebAPI : MonoBehaviour
    {
        [SerializeField] private bool _isGameWithWebSocket;
        public Action<LoadData> LoadData;
        public static event Action<DateTime> GetServerTimeEvent;

        private bool _isSubscribe;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            SubscribeToEvents(true);
        }


        private void OnDestroy()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                if (_isSubscribe) return;
                _isSubscribe = true;
                WebSocketClient.ServerMessage += ParseResponse;
                WebSocketClient.ConnectedEvent += SendLoadRequest;
            }
            else
            {
                WebSocketClient.ConnectedEvent -= SendLoadRequest;
                WebSocketClient.ServerMessage -= ParseResponse;
            }

            Debugging.Log($"WebApi: Subscribe to WebSocketClient events {flag}", ColorType.Teal);
        }

        private void SendLoadRequest()
        {
            if (_isGameWithWebSocket)
            {
                Debugging.Log($"WebApi: SendLoadRequest", ColorType.Teal);
                WebSocketClient.SendMessageToServer("{\"request\": \"load_user\"}");
            }
            else
            {
                var loadData = new LoadData
                {
                    data = new RequestData(),
                    time = DateTime.UtcNow
                };
                LoadData?.Invoke(loadData);
            }
        }

        public static void GetServerTime()
        {
            WebSocketClient.SendMessageToServer("{\"request\": \"get_time\"}");
        }
    }
}