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


    private void Awake() {
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

    public void Pick()
    {
        SpawnObjectServerRpc();
        SetPickableObjectParent(_playerController.transform);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc()
    {
        SpawnObjectClientRpc();
    }

    [ClientRpc]
    private void SpawnObjectClientRpc()
    {
        Transform boxTransform = Instantiate(boxPrefab);
        _box = boxTransform.GetComponent<PickableObject>(); 
        // NetworkObject boxNetworkObject = _box.GetNetworkObject();
        // boxNetworkObject.Spawn(true);
    }
    
    public void SetPickableObjectParent(Transform pickableObjectParent) {
        SetPickableObjectParentServerRpc(pickableObjectParent.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPickableObjectParentServerRpc(NetworkObjectReference pickableObjectParentNetworkObjectReference) {
        SetPickableObjectParentClientRpc(pickableObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetPickableObjectParentClientRpc(NetworkObjectReference pickableObjectParentNetworkObjectReference) {
        pickableObjectParentNetworkObjectReference.TryGet(out NetworkObject pickableObjectParentNetworkObject);

        _box.GetFollowTransform().SetTargetTransform(pickableObjectParentNetworkObject.GetComponent<PlayerController>().GetHoldingPointTransform());
        // _followTransform.SetTargetTransform(pickableObjectParentNetworkObject.transform);
    }
}
