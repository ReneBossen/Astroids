using UnityEngine;

namespace Assets.Scripts
{
    public class ExplosionEffect : MonoBehaviour
    {
        private ParticleSystem _effect;

        private void Awake()
        {
            _effect = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            _effect.Play();
        }
    }
}
