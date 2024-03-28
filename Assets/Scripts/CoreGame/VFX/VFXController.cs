using SO;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CoreGame
{
    public class VFXController: IInitializable
    {
        private ParticleSystem _snowFall;
        private ParticleSystem _bigSnowFall;
        private VFXConfig _vfxConfig;
        
        [Inject]
        public VFXController(IObjectResolver resolver)
        {
            _vfxConfig = resolver.Resolve<VFXConfig>();
        }
        
        public void PlaySmallSnowfall()
        {
            if (_snowFall == null)
            {
                var data = _vfxConfig.GetData(ParticleType.Snowfall_Small);
                _snowFall = Object.Instantiate(data.Prefab, data.Offset, data.Prefab.transform.rotation);
            }
            else
            {
                _snowFall.Play();
            }
        }

        public void PlayBigSnowfall()
        {
            if (_bigSnowFall == null)
            {
                var data = _vfxConfig.GetData(ParticleType.Snowfall_Big);
                _bigSnowFall = Object.Instantiate(data.Prefab, data.Offset, data.Prefab.transform.rotation);
            }
            else
            {
                _bigSnowFall.Play();
            }
        }

        public void StopAllSnowfall()
        {
            if (_snowFall != null)
            {
                Object.Destroy(_snowFall.gameObject);
            }

            if (_bigSnowFall != null)
            {
                Object.Destroy(_bigSnowFall.gameObject);
            }
        }
        
        public void Initialize()
        {
            Debug.Log("VFX Controller initialized");
        }
    }
}