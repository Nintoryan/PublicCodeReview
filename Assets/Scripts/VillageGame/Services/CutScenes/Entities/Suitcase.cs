using System.Collections;
using UnityEngine;
using Util;

namespace VillageGame.Services.CutScenes.Entities
{
    public class Suitcase : MonoBehaviour
    {
        [SerializeField] private GameObject _body;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _openSprite, _closeSprite;
        [SerializeField] private ParticleSystem[] _openParticles;
        
        public void SetOpenSprite()
        {
            _spriteRenderer.sprite = _openSprite;
        }

        public void PlayOpenParticle()
        {
            foreach (var openParticle in _openParticles)
            {
                openParticle.Play();
            }
        }
        public void SetCloseSprite()
        {
            _spriteRenderer.sprite = _closeSprite;
        }

        public void EnableSuitcase()
        {
            _body.SetActive(true);
        }

        public void DisableSuitcase(float delay = 0)
        {
            StartCoroutine(DisableWithDelay(delay));
        }

        private IEnumerator DisableWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _body.SetActive(false);
        }
    }
    
}