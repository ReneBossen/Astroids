using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class GameObjectHandler : MonoBehaviour
    {
        public static GameObjectHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
        }

        public static IEnumerator DisableAfterTime(GameObject prefab, float time)
        {
            yield return new WaitForSeconds(time);
            prefab.SetActive(false);
        }
    }
}
