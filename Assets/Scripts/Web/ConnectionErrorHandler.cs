using UnityEngine;

namespace Web
{
    public class ConnectionErrorHandler : MonoBehaviour
    {
        private static ConnectionErrorHandler _instance;

        [SerializeField] private GameObject _body;
        
        
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                _body.SetActive(false);
            }
        }

        public static void Show()
        {
            if(_instance != null)
                _instance._body.SetActive(true);
        }

        public static void Hide()
        {
            if(_instance != null)
                _instance._body.SetActive(false);
        }
    }
}