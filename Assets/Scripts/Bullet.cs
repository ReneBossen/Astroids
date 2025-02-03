using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        public event EventHandler<BulletHitEventArgs> OnBulletHit;
        public class BulletHitEventArgs : EventArgs
        {
            public GameObject Bullet { get; set; }
        }

        [SerializeField] private float _speed;

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime * _speed);
        }

        private void OnCollisionEnter2D()
        {
            OnBulletHit?.Invoke(this, new BulletHitEventArgs
            {
                Bullet = gameObject
            });
        }
    }
}