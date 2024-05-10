using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;


    private void Awake() {
        createGameButton.onClick.AddListener(() => {
            MultiplayerController.Instance.StartHost();
            GameManager.LoadNetwork(GameManager.Scene.CharactersLobbyScene);
            // GameManager.LoadSceneNetwork(GameManager.Scene.CharactersLobbyScene);
        });
        joinGameButton.onClick.AddListener(() => {
            MultiplayerController.Instance.StartClient();
        });
    }
}
