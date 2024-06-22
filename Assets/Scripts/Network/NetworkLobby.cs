using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class NetworkLobby : MonoBehaviour
{
    public static NetworkLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private Lobby joinedLobby;
    private float heartbeatTimer;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }
    
    private void Update() 
    {
        HandleHeartbeat();
    }


    private void HandleHeartbeat() 
    {
        if (IsLobbyHost()) 
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f) 
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost() 
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    
    private async void InitializeUnityAuthentication() 
    {
        if (UnityServices.State != ServicesInitializationState.Initialized) 
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerController.MAX_PLAYER_AMOUNT, new CreateLobbyOptions {
                IsPrivate = isPrivate,
            });
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, 
                new UpdateLobbyOptions 
                {
                    Data = new Dictionary<string, DataObject> 
                    {
                        {
                            KEY_RELAY_JOIN_CODE , new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                        }
                    }
                }
            );
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "udp"));

            MultiplayerController.Instance.StartHost();
            await GameManager.LoadSceneNetwork(GameManager.Scene.CharactersLobbyScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
        
    }
    
    public async void DeleteLobby() 
    {
        if (joinedLobby != null) 
        {
            try 
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            } 
            catch (LobbyServiceException e) 
            {
                Debug.Log(e);
            }
        }
    }
    
    
    public async void QuickJoin() 
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try 
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            if (joinAllocation != null) 
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "udp"));
                MultiplayerController.Instance.StartClient();
            } 
            else 
            {
                Debug.LogError("JoinAllocation is null.");
                OnJoinFailed?.Invoke(this, EventArgs.Empty);
            }
        } 
        catch (LobbyServiceException e) {
            Debug.Log(e);
            CreateLobby("Lobby" + UnityEngine.Random.Range(0, 1000).ToString(), false);
            // OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public async void JoinWithCode(string lobbyCode) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            if (joinAllocation != null) 
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "udp"));
                MultiplayerController.Instance.StartClient();
            } 
            else 
            {
                Debug.LogError("JoinAllocation is null.");
                OnJoinFailed?.Invoke(this, EventArgs.Empty);
            }
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    
    
    public async void LeaveLobby() 
    {
        if (joinedLobby != null)
        {
            try 
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            } 
            catch (LobbyServiceException e) 
            {
                Debug.Log(e);
            }
        }
    }
    
    public async void KickPlayer(string playerId) 
    {
        if (IsLobbyHost()) 
        {
            try 
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } 
            catch (LobbyServiceException e) 
            {
                Debug.Log(e);
            }
        }
    }
    
    public Lobby GetLobby() 
    {
        return joinedLobby;
    }
    
    
    // Relay Functionalities
    private async Task<Allocation> AllocateRelay() 
    {
        try 
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerController.MAX_PLAYER_AMOUNT - 1);

            return allocation;
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) 
    {
        try 
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) 
    {
        try 
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
            return default;
        }
    }

}
