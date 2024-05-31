using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RewardsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resources;
    // [SerializeField] private TextMeshProUGUI team2Score;
    [SerializeField] private Button playAgainButton;


    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            NetworkLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            MultiplayerController.Instance.Restart();
            GameManager.LoadScene(GameManager.Scene.CampusScene);
        });
    }

    public void Show() {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
