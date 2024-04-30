using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class MultiplayerController : NetworkBehaviour
{
    public Transform boxPrefab;
    public List<Transform> resourceDeliveryPathPoints;
    public Transform resourceBoxPrefab;
    
    private GameManager _gameManager;
    private ClashArenaController _clashArenaController;

    

    private static MultiplayerController _instance;
    public static MultiplayerController Instance => _instance;

    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
        }
        print(resourceDeliveryPathPoints.Count);
        _clashArenaController = ClashArenaController.Instance;
        _gameManager = GameManager.Instance;
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

        ClearObjectOnParentClientRpc(pickableObjectNetworkObjectReference);

        pickableObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearObjectOnParentClientRpc(NetworkObjectReference pickableObjectNetworkObjectReference) {
        pickableObjectNetworkObjectReference.TryGet(out NetworkObject pickableObjectNetworkObject);
        PickableObject pickableObject = pickableObjectNetworkObject.GetComponent<PickableObject>();

        pickableObject.ClearObjectOnParent();
    }

    public void SpawnResourceBoxOnDeliveryPath()
    {
        SpawnResourceBoxOnDeliveryPathServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnResourceBoxOnDeliveryPathServerRpc()
    {
        Transform resourceBoxTransform = Instantiate(resourceBoxPrefab, resourceDeliveryPathPoints[0].position, Quaternion.identity);
        ObjectDelivery box = resourceBoxTransform.GetComponent<ObjectDelivery>();
        NetworkObject boxNetworkObject = box.GetNetworkObject();
        boxNetworkObject.Spawn(true);
        // SpawnResourceBoxOnDeliveryPathClientRpc();
    }

    // [ClientRpc]
    // private void SpawnResourceBoxOnDeliveryPathClientRpc()
    // {
    //     Transform resourceBoxTransform = Instantiate(resourceBoxPrefab, resourceDeliveryPathPoints[0].position, Quaternion.identity);
    //     ObjectDelivery box = resourceBoxTransform.GetComponent<ObjectDelivery>();
    //     NetworkObject boxNetworkObject = box.GetNetworkObject();
    //     boxNetworkObject.Spawn(true);
    // }

    
}
