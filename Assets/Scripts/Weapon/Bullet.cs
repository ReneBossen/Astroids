using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class Bullet : NetworkBehaviour
    {
        //public event EventHandler<BulletHitEventArgs> OnBulletHit;
        //public class BulletHitEventArgs : EventArgs
        //{
        //    public GameObject Bullet { get; set; }
        //}

        [SerializeField] private float _speed;
        [SerializeField] private float _bulletLifeTime;

        private Bullet bulletScript;

        [SyncVar(hook = nameof(OnActiveChanged))]
        public bool IsActive;

        private void Awake()
        {
            bulletScript = GetComponent<Bullet>();
        }

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime * _speed);
        }

        private void OnEnable()
        {
            if (isServer)
            {
                StartCoroutine(DisableSelf());
            }
        }

        [Server]
        private IEnumerator DisableSelf()
        {
            yield return new WaitForSeconds(_bulletLifeTime);
            bulletScript.IsActive = false;
        }

        private void OnActiveChanged(bool oldValue, bool newValue)
        {
            gameObject.SetActive(newValue);
        }

        //[ServerCallback]
        //private void OnCollisionEnter2D()
        //{
        //    OnBulletHit?.Invoke(this, new BulletHitEventArgs
        //    {
        //        Bullet = gameObject
        //    });
        //}
    }
}