using System;
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
    private bool _isConveyorBeltStopped = false;
    private int _team1Score;
    private int _team2Score;
    
    private GameManager _gameManager;
    private ClashArenaController _clashArenaController;
    private ClashSceneUI _clashSceneUI;
    [HideInInspector] public float conveyorButtonCooldownTimer = 0;

    public bool GetIsConveyorBeltStopped() => _isConveyorBeltStopped;

    public void SetIsConveyorBeltStopped(bool isConveyorBeltStopped)
    {
        _isConveyorBeltStopped = isConveyorBeltStopped;
    }

    private static MultiplayerController _instance;
    public static MultiplayerController Instance => _instance;

    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
        }
        // print(resourceDeliveryPathPoints.Count);
        _clashArenaController = ClashArenaController.Instance;
        _gameManager = GameManager.Instance;
        _clashSceneUI = ClashSceneUI.Instance;
        _team1Score = 0;
        _team2Score = 0;
        _isConveyorBeltStopped = false;
    }
    
    
    
    private void Start() {
        // StartHost();
    }
    
    public void StartHost() {
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }


    // id should be either 1 for team1 or 2 for team2
    public void IncreaseTeamScore(int teamId) 
    {
        if (teamId == 1)
        {
            _team1Score++;
            _clashSceneUI.SetScore(1, _team1Score);
        }
        else if (teamId == 2)
        {
            _team2Score++;
            _clashSceneUI.SetScore(2, _team2Score);
        }
        Debug.Log("Team1: " + _team1Score + " - Team2: " + _team2Score);
    }
    
    public int GetTeamScore(int teamId) 
    {
        if (teamId == 1)
        {
            return _team1Score;
        }
        else if (teamId == 2)
        {
            return _team2Score;
        }

        return 0;
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
        boxTransform.GetComponent<Renderer>().material.color = objectParent.GetTeamColor(); // Set the color of boxes with respect to their team
        NetworkObject boxNetworkObject = boxTransform.GetComponent<NetworkObject>();
        boxNetworkObject.Spawn(true);

        PickableObject box = boxTransform.GetComponent<PickableObject>();
        box.SetPickableObjectParent(objectParent);
        SpawnObjectClientRpc(boxNetworkObject, pickableObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SpawnObjectClientRpc(NetworkObjectReference boxNetworkObjectReference, NetworkObjectReference pickableObjectParentNetworkObjectReference)
    {
        
        pickableObjectParentNetworkObjectReference.TryGet(out NetworkObject pickableObjectParentNetworkObject);
        boxNetworkObjectReference.TryGet(out NetworkObject boxNetworkObject);
        IParent<PickableObject> objectParent = pickableObjectParentNetworkObject.GetComponent<IParent<PickableObject>>();
        boxNetworkObject.GetComponent<Renderer>().material.color = objectParent.GetTeamColor(); // Set the color of boxes with respect to their team
        
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

    
    public void SpawnResourceBoxOnDeliveryPath(IParent<PickableObject> objectParent)
    {
        SpawnResourceBoxOnDeliveryPathServerRpc(objectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnResourceBoxOnDeliveryPathServerRpc(NetworkObjectReference pickableObjectParentNetworkObjectReference)
    {
        if (!pickableObjectParentNetworkObjectReference.TryGet(out NetworkObject pickableObjectParentNetworkObject))
        {
            Debug.LogError("Failed to get pickableObjectParentNetworkObject from reference.");
            return;
        }

        IParent<PickableObject> objectParent = pickableObjectParentNetworkObject.GetComponent<IParent<PickableObject>>();
        Transform resourceBoxTransform = Instantiate(resourceBoxPrefab, resourceDeliveryPathPoints[0].position, Quaternion.identity);
        ObjectDelivery box = resourceBoxTransform.GetComponent<ObjectDelivery>();
        NetworkObject boxNetworkObject = box.GetNetworkObject();
        boxNetworkObject.Spawn(true);
        box.SetResourceObjectAttributes(objectParent.GetTeamCharacteristics());
    }
    
    // [ClientRpc]
    // private void SpawnResourceBoxOnDeliveryPathClientRpc(NetworkObjectReference a, NetworkObjectReference pickableObjectParentNetworkObjectReference)
    // {
    //     
    //     pickableObjectParentNetworkObjectReference.TryGet(out NetworkObject pickableObjectParentNetworkObject);
    //     a.TryGet(out NetworkObject box);
    //     // Transform resourceBoxTransform = Instantiate(resourceBoxPrefab, resourceDeliveryPathPoints[0].position, Quaternion.identity);
    //     // resourceBoxTransform.GetComponent<Renderer>().material.color = objectParent.GetTeamColor(); // Set the color of boxes with respect to their team
    //     // ObjectDelivery box = resourceBoxTransform.GetComponent<ObjectDelivery>();
    //     // NetworkObject boxNetworkObject = box.GetNetworkObject();
    //     // boxNetworkObject.Spawn(true);
    //     IParent<PickableObject> objectParent = pickableObjectParentNetworkObject.GetComponent<IParent<PickableObject>>();
    //     try
    //     {
    //         box.GetComponent<Renderer>().material.color = objectParent.GetTeamColor(); // Set the color of boxes with respect to their team
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //     }   
    // }

    // [ClientRpc]
    // private void SpawnResourceBoxOnDeliveryPathClientRpc()
    // {
    //     Transform resourceBoxTransform = Instantiate(resourceBoxPrefab, resourceDeliveryPathPoints[0].position, Quaternion.identity);
    //     ObjectDelivery box = resourceBoxTransform.GetComponent<ObjectDelivery>();
    //     NetworkObject boxNetworkObject = box.GetNetworkObject();
    //     boxNetworkObject.Spawn(true);
    // }

    
}
