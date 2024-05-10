using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;


    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        MultiplayerController.Instance.OnFailedToJoinGame += MultiplayerController_OnFailedToJoinGame;
        Hide();
    }

    private void MultiplayerController_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;
        if (messageText.text == "")
        {
            messageText.text = "Connection Failed";
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MultiplayerController.Instance.OnFailedToJoinGame -= MultiplayerController_OnFailedToJoinGame;
    }
}
