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
    }
}