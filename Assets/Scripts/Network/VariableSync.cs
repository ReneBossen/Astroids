using Mirror;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class VariableSync : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnActiveChanged))]
        public bool IsActive;

        void OnActiveChanged(bool oldValue, bool newValue)
        {
            gameObject.SetActive(newValue);
        }
    }
}