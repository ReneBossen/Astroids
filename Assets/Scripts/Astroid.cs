using Assets.Scripts.Interfaces;
using Assets.Scripts.Network;
using Mirror;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Astroid : NetworkBehaviour, ISyncVariables
    {
        public event EventHandler<OnAstroidHitEventArgs> OnAstroidHit;
        public event EventHandler<OnAstroidHitEventArgs> OnPlayerHit;

        public class OnAstroidHitEventArgs : EventArgs
        {
            public GameObject Astroid;
            public int Value;
        }

        public bool IsActive
        {
            get => SyncComponent.IsActive;
            set => SyncComponent.IsActive = value;
        }

        [SerializeField] private float _speed;
        [SerializeField] private int _scoreValue;

        [SyncVar(hook = nameof(OnRotationChanged))]
        private float zRotation;

        public VariableSync SyncComponent { get; private set; }

        private void Awake()
        {
            SyncComponent = GetComponent<VariableSync>();
        }

        private void OnEnable()
        {
            if (isServer)
            {
                zRotation = Random.Range(0, 360);
            }
        }

        private void OnRotationChanged(float oldRotation, float newRotation)
        {
            transform.Rotate(0, 0, newRotation);
        }

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime * _speed);
        }

        [ServerCallback]
        private void OnCollisionEnter2D(Collision2D collider)
        {
            if (collider.gameObject.GetComponent<Player.Player>() != null)
            {
                OnPlayerHit?.Invoke(this, new OnAstroidHitEventArgs
                {
                    Astroid = gameObject,
                    Value = _scoreValue
                });
            }

            OnAstroidHit?.Invoke(this, new OnAstroidHitEventArgs
            {
                Astroid = gameObject,
                Value = _scoreValue
            });
        }
    }
}