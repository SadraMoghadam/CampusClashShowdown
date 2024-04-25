using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PickableObjectGenerator : NetworkBehaviour
{
    public Transform boxPrefab;

    public void Pick()
    {
        SpawnObjectServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc()
    {
        
        Transform boxTransform = Instantiate(boxPrefab);
        NetworkObject box = boxTransform.GetComponent<NetworkObject>();
        box.Spawn(true);
    }
}
