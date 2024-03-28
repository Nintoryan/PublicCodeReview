using UnityEngine;

namespace VillageGame.Logic.Buildings
{
    public class SpriteArrToggle: MonoBehaviour
    {
        [SerializeField] private GameObject[] _sprites;
        
        public void ActiveSprite(bool isActive)
        {
            foreach (var sprite in _sprites)
            {
                gameObject.SetActive(isActive);
            }
        }
    }
}