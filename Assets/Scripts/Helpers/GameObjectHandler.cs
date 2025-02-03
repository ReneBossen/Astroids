using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class GameObjectHandler : MonoBehaviour
    {
        public static IEnumerator DisableAfterTime(GameObject prefab, float time)
        {
            yield return new WaitForSeconds(time);
            prefab.SetActive(false);
        }

        public static void RepositionGameObject(GameObject value, Vector2 position)
        {
            value.transform.position = position;
        }
    }
}