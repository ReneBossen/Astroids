using Mirror;
using UnityEngine;

public class NameSync : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string objectName;

    void OnNameChanged(string oldName, string newName)
    {
        gameObject.name = newName;
    }
}