using Assets.Scripts.Interfaces;
using Assets.Scripts.Network;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class Bullet : NetworkBehaviour, ISyncVariables
    {
        //public event EventHandler<BulletHitEventArgs> OnBulletHit;
        //public class BulletHitEventArgs : EventArgs
        //{
        //    public GameObject Bullet { get; set; }
        //}

        public bool IsActive
        {
            get => SyncComponent.IsActive;
            set => SyncComponent.IsActive = value;
        }

        public VariableSync SyncComponent { get; private set; }

        [SerializeField] private float _speed;
        [SerializeField] private float _bulletLifeTime;

        private Bullet bulletScript;

        private void Awake()
        {
            bulletScript = GetComponent<Bullet>();
            SyncComponent = GetComponent<VariableSync>();
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