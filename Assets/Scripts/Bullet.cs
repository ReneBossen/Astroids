using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime * _speed);
        }

        private void OnCollisionEnter2D(Collision2D collider)
        {
            DisableBullet();
        }

        private void DisableBullet()
        {
            gameObject.SetActive(false);
        }
    }
}