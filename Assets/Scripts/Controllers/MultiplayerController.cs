using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MultiplayerController : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    private bool _isConveyorBeltStopped = false;
    private int _team1Score;
    private int _team2Score;
    
    private NetworkList<PlayerData> playerDataNetworkList;
    
    // private GameManager _gameManager;
    // private ClashArenaController _clashArenaController;
    // private ClashSceneUI _clashSceneUI;
    [HideInInspector] public float conveyorButtonCooldownTimer = 0;
    
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private static MultiplayerController _instance;
    public static MultiplayerController Instance => _instance;

    private string _playerName;
    
    
    private void Awake() 
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
        // print(resourceDeliveryPathPoints.Count);
        // _clashArenaController = ClashArenaController.Instance;
        // _gameManager = GameManager.Instance;
        // _clashSceneUI = ClashSceneUI.Instance;
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        _team1Score = 0;
        _team2Score = 0;
        _isConveyorBeltStopped = false;
        _playerName =
            PlayerPrefsManager.GetString(PlayerPrefsKeys.PlayerName, "Player" + UnityEngine.Random.Range(100, 1000));
    }
    
    
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() 
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }
      
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        if (playerDataNetworkList == null)
        {
            playerDataNetworkList = new NetworkList<PlayerData>();
        }
        PlayerData newPlayerData = new PlayerData
        {
            clientId = clientId,
        };
        playerDataNetworkList.Add(newPlayerData);
        SetPlayerMeshServerRpc(PlayerPrefsManager.GetHeadAndBodyMeshIndices().Item1, PlayerPrefsManager.GetHeadAndBodyMeshIndices().Item2);
        // ClientConnectedCallbackServerRpc(clientId);
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId) {
                // Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }
    // [ServerRpc(RequireOwnership = false)]
    // private void ClientConnectedCallbackServerRpc(ulong clientId)
    // {
    //     playerDataNetworkList.Add(new PlayerData {
    //         clientId = clientId,
    //     }); 
    // }
    

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) 
    {
        if (SceneManager.GetActiveScene().name != GameManager.Scene.CharactersLobbyScene.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public void StartClient() 
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }
    
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) 
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerMeshServerRpc(PlayerPrefsManager.GetHeadAndBodyMeshIndices().Item1, PlayerPrefsManager.GetHeadAndBodyMeshIndices().Item2);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerMeshServerRpc(int headMeshId, int bodyMeshId, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.headMeshId = headMeshId;
        playerData.bodyMeshId = bodyMeshId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) 
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    // id should be either 1 for team1 or 2 for team2
    public void IncreaseTeamScore(int teamId) 
    {
        if (teamId == 1)
        {
            _team1Score++;
            ClashSceneUI.Instance.SetScore(1, _team1Score);
        }
        else if (teamId == 2)
        {
            _team2Score++;
            ClashSceneUI.Instance.SetScore(2, _team2Score);
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
        
        Transform boxTransform = Instantiate(ClashArenaController.Instance.boxPrefab);
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
        Transform resourceBoxTransform = Instantiate(ClashArenaController.Instance.resourceBoxPrefab, ClashArenaController.Instance.resourceDeliveryPathPoints[0].position, Quaternion.identity);
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

    
    public bool IsPlayerIndexConnected(int playerIndex) 
    {
        Debug.Log("BBBBBBBBBBBBBBBBBBBB" + playerDataNetworkList.Count);
        return playerIndex < playerDataNetworkList.Count;
    }
    
    public int GetPlayerDataIndexFromClientId(ulong clientId) 
    {
        for (int i=0; i< playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId) 
    {
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.clientId == clientId) {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData() 
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) 
    {
        return playerDataNetworkList[playerIndex];
    }
    
    public Mesh GetPlayerHeadMesh(int headMeshId) 
    {
        return GameManager.Instance.playerMeshes.HeadMeshes[headMeshId];
    }
    
    public Mesh GetPlayerBodyMesh(int bodyMeshId) 
    {
        return GameManager.Instance.playerMeshes.BodyMeshes[bodyMeshId];
    }

    public void KickPlayer(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
    
    
    public bool GetIsConveyorBeltStopped() => _isConveyorBeltStopped;

    public void SetIsConveyorBeltStopped(bool isConveyorBeltStopped)
    {
        _isConveyorBeltStopped = isConveyorBeltStopped;
    }
    
    public string GetPlayerName() {
        return _playerName;
    }

    public void SetPlayerName(string playerName) {
        _playerName = playerName;
        PlayerPrefsManager.SetString(PlayerPrefsKeys.PlayerName, playerName);
    }
}
