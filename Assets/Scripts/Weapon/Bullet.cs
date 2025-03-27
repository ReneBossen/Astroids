using Assets.Scripts.Interfaces;
using Assets.Scripts.Network;
using JetBrains.Annotations;
using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Assets.Scripts.Astroids.Astroid;

namespace Assets.Scripts.Weapon
{
    public class Bullet : NetworkBehaviour, ISyncVariables
    {
        public event EventHandler<OnBulletDisableEventArgs> OnPlayerHit;
        public event EventHandler<OnBulletDisableEventArgs> OnDisable;

        public class OnBulletDisableEventArgs : EventArgs
        {
            public GameObject Bullet;
        }

        public bool IsActive
        {
            get => SyncComponent.IsActive;
            set => SyncComponent.IsActive = value;
        }

        public VariableSync SyncComponent { get; private set; }
        [CanBeNull] public NetworkIdentity ShooterIdentity;

        [SerializeField] private float _speed;
        [SerializeField] private float _bulletLifeTime;

        private void Awake()
        {
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
            OnDisable?.Invoke(this, new OnBulletDisableEventArgs
            {
                Bullet = gameObject
            });

            ShooterIdentity = null;
            IsActive = false;
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (ShooterIdentity != null && collider.gameObject == ShooterIdentity.gameObject) //Nullable fejl??
                return;

            if (collider.gameObject.TryGetComponent(out Player.Player _))
            {
                OnPlayerHit?.Invoke(this, new OnBulletDisableEventArgs
                {
                    Bullet = gameObject
                });
            }
            else
            {
                OnDisable?.Invoke(this, new OnBulletDisableEventArgs
                {
                    Bullet = gameObject
                });
            }

            ShooterIdentity = null;
            IsActive = false;
        }
    }
}