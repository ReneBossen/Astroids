using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Astroid : MonoBehaviour
    {
        public event EventHandler<OnAstroidHitEventArgs> OnAstroidHit;

        public class OnAstroidHitEventArgs : EventArgs
        {
            public GameObject Astroid;
            public int Value;
        }

        [SerializeField] private float _speed;
        [SerializeField] private int _scoreValue;

        private void OnEnable()
        {
            transform.Rotate(0, 0, Random.Range(0, 360));
        }

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime * _speed);
        }

        private void OnCollisionEnter2D(Collision2D collider)
        {
            OnAstroidHit?.Invoke(this, new OnAstroidHitEventArgs
            {
                Astroid = gameObject,
                Value = _scoreValue
            });
        }
    }
}