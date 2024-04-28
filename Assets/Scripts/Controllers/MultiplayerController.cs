using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerController : NetworkBehaviour
{
    public Transform boxPrefab;
    private ClashArenaController _clashArenaController;

    
    public static MultiplayerController Instance { get; private set; }

    private void Awake() {
        Instance = this;
        _clashArenaController = ClashArenaController.Instance;
    }

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

    public void SpawnResourceBoxOnDeliveryPath()
    {
        SpawnResourceBoxOnDeliveryPathServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnResourceBoxOnDeliveryPathServerRpc()
    {
        SpawnResourceBoxOnDeliveryPathClientRpc();
    }

    [ClientRpc]
    private void SpawnResourceBoxOnDeliveryPathClientRpc()
    {
        Transform resourceBoxTransform = Instantiate(_clashArenaController.resourceBoxPrefab, _clashArenaController.resourcePathPoints[0].position, Quaternion.identity);
        ObjectDelivery _box = resourceBoxTransform.GetComponent<ObjectDelivery>();
        NetworkObject boxNetworkObject = _box.GetNetworkObject();
        boxNetworkObject.Spawn(true);
    }
    
    
}
