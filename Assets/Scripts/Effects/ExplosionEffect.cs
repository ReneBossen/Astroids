using UnityEngine;

namespace Assets.Scripts.Effects
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
