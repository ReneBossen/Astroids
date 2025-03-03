using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class GameObjectHandler : NetworkBehaviour
    {
        public static GameObjectHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        [Command]
        public void RepositionGameObject(GameObject value, Vector2 position)
        {
            value.transform.position = position;
        }
    }
}