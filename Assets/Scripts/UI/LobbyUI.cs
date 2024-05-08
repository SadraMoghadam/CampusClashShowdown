using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour 
{
    
    public Button hostButton;

    public Button clientButton;
    
    private void Start() {
        ClashArenaController.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            ClashArenaController.Instance.OnReadyButtonClicked();
        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            ClashArenaController.Instance.OnReadyButtonClicked();
        });

        Show();
    }

    private void KitchenGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e) {
        if (ClashArenaController.Instance.IsLocalPlayerReady()) {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}