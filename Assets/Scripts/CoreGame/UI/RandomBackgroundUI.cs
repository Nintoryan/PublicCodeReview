using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CoreGame.UI
{
    [RequireComponent(typeof(Image))]
    public class RandomBackgroundUI : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _sprites;

        private void Start()
        {
            if (_sprites.Count > 0)
            {
                GetComponent<Image>().sprite = _sprites[Random.Range(0, _sprites.Count)];
            }
        }
    }
}