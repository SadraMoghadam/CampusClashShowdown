using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLobbyUI : MonoBehaviour
{
    
    [SerializeField] private Button readyButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;


    private void Awake() {
        GameManager.Instance.AddButtonsSound();
        mainMenuButton.onClick.AddListener(() => {
            NetworkLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            GameManager.LoadScene(GameManager.Scene.CampusScene);
        });
        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start() {
        Lobby lobby = NetworkLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
