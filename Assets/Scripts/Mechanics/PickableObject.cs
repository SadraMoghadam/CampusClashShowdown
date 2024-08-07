using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PickableObject : NetworkBehaviour
{
    [SerializeField] private GameObject boxVfx;
    private FollowTransform _followTransform;
    private NetworkObject _networkObject;
    private IParent<PickableObject> _objectParent;

    private void Awake()
    {
        _followTransform = GetComponent<FollowTransform>();
        _networkObject = GetComponent<NetworkObject>();
    }

    public IParent<PickableObject> GetObjectParent() {
        return _objectParent;
    }
    
    public void SetPickableObjectParent(IParent<PickableObject> pickableObjectParent) {
        SetPickableObjectParentServerRpc(pickableObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPickableObjectParentServerRpc(NetworkObjectReference pickableObjectParentNetworkObjectReference) {
        SetPickableObjectParentClientRpc(pickableObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetPickableObjectParentClientRpc(NetworkObjectReference pickableObjectParentNetworkObjectReference) {
        
        pickableObjectParentNetworkObjectReference.TryGet(out NetworkObject pickableObjectParentNetworkObject);
        IParent<PickableObject> objectParent = pickableObjectParentNetworkObject.GetComponent<IParent<PickableObject>>();
        if (_objectParent != null)
        {
            _objectParent.ClearChild();
        }
        _objectParent = objectParent;
        if (objectParent.HasChild())
        {
            return;
        }
        objectParent.SetChild(this);
        ClashVFXContainer.ChangeParticlesColor(boxVfx, objectParent.GetTeamColor());
        _followTransform.SetTargetTransform(objectParent.GetChildFollowTransform());
    }
    
    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void ClearObjectOnParent() {
        _objectParent.ClearChild();
    }
    
    
    public static void SpawnObject(IParent<PickableObject> objectParent) {
        MultiplayerController.Instance.SpawnObject(objectParent);
    }

    public static void DestroyObject(PickableObject pickableObject) {
        MultiplayerController.Instance.DestroyObject(pickableObject);
    }

    
    
    
}
