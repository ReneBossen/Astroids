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
    }
}