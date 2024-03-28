using UnityEngine;
using UnityEngine.UI;

namespace VillageGame.UI.Indicators
{
    public class FillArea : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetValue(float current, float max)
        {
            if (max == 0)
            {
                Debug.LogWarning($"Max in fill area {gameObject.name} was attempt to set to 0");
                _image.fillAmount = 0;
                return;
            }
            _image.fillAmount = current / max;
        }

        
        public void SetValue(float value)
        {
            if (_image.gameObject.activeInHierarchy)
            {
                _image.fillAmount = value;
            }
        }
    }
}