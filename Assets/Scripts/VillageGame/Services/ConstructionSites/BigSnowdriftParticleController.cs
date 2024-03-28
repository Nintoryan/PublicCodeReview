using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VillageGame.Services.Snowdrifts
{
    public class BigSnowdriftParticleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _ecpicenter;
        [SerializeField] private List<ParticleSystem> _rightSmallParticles;
        [SerializeField] private List<ParticleSystem> _leftSmallParticles;

        [SerializeField] private Vector2 _ellipseSize;
        [SerializeField] public float _speed = 2f;

        private float _timeRight;
        private float _timeLeft;

        private bool _isAnimation;

        private void Update()
        {
            if (!_isAnimation)
            {
                return;
            }
            
            for (int i = 0; i < _rightSmallParticles.Count; i++)
            {
                float phase = 360.0f / _rightSmallParticles.Count * i;
                MoveRightOnEllipse(_rightSmallParticles[i], phase);
            }
            for (int i = 0; i < _leftSmallParticles.Count; i++)
            {
                float phase = 360.0f / _leftSmallParticles.Count * i;
                MoveLeftOnEllipse(_leftSmallParticles[i], phase);
            }
        }

        public void StartAnimation()
        {
            _isAnimation = true;
            foreach (var particle in GetAllParticles())
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }

            StartCoroutine(StopAllParticlesWithDelay(1));
        }

        private void MoveRightOnEllipse(ParticleSystem particleSystem, float phase)
        {
            _timeRight += Time.deltaTime * _speed;

            var angle = phase + _timeRight * Mathf.Deg2Rad;

            var x = _ellipseSize.x * Mathf.Cos(angle);
            var y = _ellipseSize.y * Mathf.Sin(angle);
            particleSystem.transform.position = new Vector3(x, y, 0f);
        }

        private void MoveLeftOnEllipse(ParticleSystem particleSystem, float phase)
        {
            _timeLeft += Time.deltaTime * _speed;

            // Измените знак для левых частиц
            var angle = phase - _timeLeft * Mathf.Deg2Rad;

            var x = _ellipseSize.x * Mathf.Cos(angle);
            var y = _ellipseSize.y * Mathf.Sin(angle);
            particleSystem.transform.position = new Vector3(x, y, 0f);
        }

        private IEnumerator StopAllParticlesWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            var allParticleSystems = GetAllParticles();
            foreach (var particle in allParticleSystems)
            {
                StopParticle(particle);
            }
            yield return new WaitForSeconds(5);
            foreach (var particle in allParticleSystems)
            {
                particle.Stop();
                var childEmission = particle.emission;
                childEmission.enabled = true;
            }
            _isAnimation = false;
        }

        private void StopParticle(ParticleSystem particleSystem)
        {
            var mainEmission = particleSystem.emission;
            mainEmission.enabled = false;

            var childParticles = particleSystem.GetComponentsInChildren<ParticleSystem>();
            foreach (var childParticle in childParticles)
            {
                var childEmission = childParticle.emission;
                childEmission.enabled = false;
            }

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            const int segments = 100;
            const float angleIncrement = 360.0f / segments;

            var previousPoint = Vector3.zero;
            
            for (var i = 0; i < segments; i++)
            {
                var angle = i * angleIncrement * Mathf.Deg2Rad;
                var x = _ellipseSize.x * Mathf.Cos(angle);
                var y = _ellipseSize.y * Mathf.Sin(angle);
                var point = new Vector3(x, y,0);

                if (previousPoint != Vector3.zero)
                {
                    Gizmos.DrawLine(previousPoint, point);
                }
                previousPoint = point;
            }
        }

        private List<ParticleSystem> GetAllParticles()
        {
            List<ParticleSystem> allParticleSystems = new List<ParticleSystem>();
            allParticleSystems.Add(_ecpicenter);
            allParticleSystems.AddRange(_rightSmallParticles);
            allParticleSystems.AddRange(_leftSmallParticles);
            return allParticleSystems;
        }
    }
}