using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NetworkLobby : MonoBehaviour
{
    public static NetworkLobby Instance { get; private set; }
    private Lobby joinedLobby;
    private float heartbeatTimer;

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
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

    private bool IsLobbyHost() {
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
            
            MultiplayerController.Instance.StartHost();
            GameManager.LoadSceneNetwork(GameManager.Scene.CharactersLobbyScene);
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
            
            MultiplayerController.Instance.StartClient();
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
            MultiplayerController.Instance.StartClient();
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

}
