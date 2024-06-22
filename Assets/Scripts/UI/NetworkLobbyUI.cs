using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private NetworkLobbyCreateUI lobbyCreateUI;


    private void Awake() 
    {
        GameManager.Instance.AddButtonsSound();
        mainMenuButton.onClick.AddListener(() => {
            NetworkLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            GameManager.LoadScene(GameManager.Scene.CampusScene);
        });
        createLobbyButton.onClick.AddListener(() => {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() => {
            NetworkLobby.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() => {
            NetworkLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });
    }
    
    // [SerializeField] private Button createGameButton;
    // [SerializeField] private Button joinGameButton;


    // private void Awake() {
    //     createGameButton.onClick.AddListener(() => {
    //         MultiplayerController.Instance.StartHost();
    //         GameManager.LoadNetwork(GameManager.Scene.CharactersLobbyScene);
    //         // GameManager.LoadSceneNetwork(GameManager.Scene.CharactersLobbyScene);
    //     });
    //     joinGameButton.onClick.AddListener(() => {
    //         MultiplayerController.Instance.StartClient();
    //     });
    // }
}
