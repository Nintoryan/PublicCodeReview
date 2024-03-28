using UnityEngine;
using UnityEngine.UI;

namespace VillageGame.UI.Panels
{
    public class LevelUpNewItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        
        public void SetIcon(Sprite icon)
        {
            _image.sprite = icon;
        }
    }
}