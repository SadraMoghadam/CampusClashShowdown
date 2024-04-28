using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PickableObjectGenerator : NetworkBehaviour
{
    public Transform boxPrefab;
    // private FollowTransform _followTransform;
    private PlayerController _playerController;
    private PickableObject _box;

    
    public static PickableObjectGenerator Instance { get; private set; }

    private void Awake() {
        Instance = this;
        StartCoroutine(WaitForPlayerControllerInitialization());
    }
    
    
    private IEnumerator WaitForPlayerControllerInitialization()
    {
        // Wait until the PlayerController instance is available
        while (PlayerController.Instance == null)
        {
            yield return null;
        }
        _playerController = PlayerController.Instance;
    }

    // public void Pick()
    // {
    //     SpawnObject();
    // }

    public void SpawnObject(IParent<PickableObject> objectParent)
    {
        SpawnObjectServerRpc(objectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc(NetworkObjectReference pickableObjectParentNetworkObjectReference)
    {
        pickableObjectParentNetworkObjectReference.TryGet(out NetworkObject pickableObjectParentNetworkObject);
        IParent<PickableObject> objectParent = pickableObjectParentNetworkObject.GetComponent<IParent<PickableObject>>();
        if (objectParent.HasChild())
        {
            return;
        }
        
        Transform boxTransform = Instantiate(boxPrefab);
        NetworkObject boxNetworkObject = boxTransform.GetComponent<NetworkObject>();
        boxNetworkObject.Spawn(true);

        PickableObject box = boxTransform.GetComponent<PickableObject>();
        box.SetPickableObjectParent(objectParent);
    }
    
    // [ClientRpc]
    // private void SpawnObjectClientRpc(NetworkObjectReference pickableObjectParentNetworkObjectReference)
    // {
    //     pickableObjectParentNetworkObjectReference.TryGet(out NetworkObject pickableObjectParentNetworkObject);
    //     IParent<PickableObject> objectParent = pickableObjectParentNetworkObject.GetComponent<IParent<PickableObject>>();
    //     pickableObjectNetworkObjectReference.TryGet(out NetworkObject pickableObjectNetworkObject);
    //     PickableObject box = pickableObjectNetworkObject.GetComponent<PickableObject>();
    //     box.SetPickableObjectParent(objectParent);
    //     // Transform boxTransform = Instantiate(boxPrefab);
    //     // _box = boxTransform.GetComponent<PickableObject>(); 
    //     // NetworkObject boxNetworkObject = _box.GetNetworkObject();
    //     // boxNetworkObject.Spawn(true);
    // }
    
    
    // public void Drop()
    // {
    //     DestroyObjectServerRpc();
    //     _box.Destroy();
    // }

    public void DestroyObject(PickableObject pickableObject)
    {
        DestroyObjectServerRpc(pickableObject.NetworkObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc(NetworkObjectReference pickableObjectNetworkObjectReference) {
        pickableObjectNetworkObjectReference.TryGet(out NetworkObject pickableObjectNetworkObject);

        if (pickableObjectNetworkObject == null) {
            return;
        }

        PickableObject pickableObject = pickableObjectNetworkObject.GetComponent<PickableObject>();

        ClearKitchenObjectOnParentClientRpc(pickableObjectNetworkObjectReference);

        pickableObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference pickableObjectNetworkObjectReference) {
        pickableObjectNetworkObjectReference.TryGet(out NetworkObject pickableObjectNetworkObject);
        PickableObject pickableObject = pickableObjectNetworkObject.GetComponent<PickableObject>();

        pickableObject.ClearKitchenObjectOnParent();
    }

    // [ServerRpc(RequireOwnership = false)]
    // private void DestroyObjectServerRpc()
    // {
    //     DestroyObjectClientRpc();
    // }
    //
    // [ClientRpc]
    // private void DestroyObjectClientRpc()
    // {
    //     if (_box != null)
    //     {
    //         // Destroy the object for all clients
    //         Destroy(_box.gameObject);
    //     }
    // }
    
    // public void ClearPickableObjectParent(Transform pickableObjectParent) {
    //     ClearPickableObjectParentServerRpc(pickableObjectParent.GetComponent<NetworkObject>());
    // }
    
    
}
