using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    private FollowTransform _followTransform;
    private NetworkObject _networkObject;

    public FollowTransform GetFollowTransform()
    {
        _followTransform = GetComponent<FollowTransform>();
        return _followTransform;
    }
    
    public NetworkObject GetNetworkObject()
    {
        _networkObject = GetComponent<NetworkObject>();
        return _networkObject;
    }
    
    public void Destroy()
    {
        // Perform any necessary cleanup
        // Destroy the object
        Destroy(gameObject);
    }
    
}
