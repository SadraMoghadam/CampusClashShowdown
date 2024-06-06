using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// This Class can be used as a singleton and has the control info and functionalities that will be needed in
/// the ClashArena scenes 
/// </summary>
public class ClashArenaController : NetworkBehaviour
{
    public List<Transform> spawnLocations;
    public Transform boxPrefab;
    public List<Transform> resourceDeliveryPathPoints;
    public Transform resourceBoxPrefab;
    public List<KeyBindingUI> keyBindingUis;
    public KeyBindingHelperUI keyBindingHelperUi;
    private static ClashArenaController _instance;
    public static ClashArenaController Instance => _instance;
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private Camera endGameCamera;
    
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPlayerReadyChanged;
    
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(180f);
    private float gamePlayingTimerMax = 180f;
    private bool isLocalGamePaused = false;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private bool autoTestGamePausedState;
    
    
    public enum ObjectType
    {
        Pressable, // able to press
        Pickable, // able to pick up and put down
        Pushable, // able to push or pull
        BlockButton,
        ConveyorButton,
    }
    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }
    


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        playerReadyDictionary = new Dictionary<ulong, bool>();
        GameManager.Instance.AudioManager.Play(SoundName.ClashTheme);
    }

    
    
    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        
        
        if (IsServer) {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }
    
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
    
    private void Update() {
        if (!IsServer) 
        {
            return;
        }

        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f) {
                    state.Value = State.GameOver;
                    GameManager.Instance.AudioManager.Stop(SoundName.ClashTheme);
                    GameManager.Instance.AudioManager.Instantplay(SoundName.GameOver, transform.position);
                    endGameCamera.gameObject.SetActive(true);
                }
                break;
            case State.GameOver:
                break;
        }
    }
    
    private void State_OnValueChanged(State previousValue, State newValue) {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnReadyButtonClicked() {
        if (state.Value == State.WaitingToStart) {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();

        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady) {
            state.Value = State.CountdownToStart;
        }
    }

    
    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }

    public bool IsWaitingToStart() {
        return state.Value == State.WaitingToStart;
    }

    public bool IsLocalPlayerReady() {
        return isLocalPlayerReady;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    
    public int GetRemainingTime() {
        return (int)gamePlayingTimer.Value;
    }
}
