using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SettingsClashUI : MonoBehaviour
{
    
    [SerializeField] private Button closeButton;
    [SerializeField] private Button keyBindingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject KeyBindingsUIGO;
    
    private GameManager _gameManager;

    private void Awake()
    {
        
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
        
        keyBindingsButton.onClick.AddListener(() =>
        {
            KeyBindingsUIGO.SetActive(true);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            NetworkLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            MultiplayerController.Instance.Restart();
            GameManager.LoadScene(GameManager.Scene.CampusScene);
        });
        
    }

    private void OnEnable()
    {
        GameManager.Instance.AudioManager.ChangeMusicVolume(.3f);
    }

    private void Hide()
    {
        GameManager.Instance.AudioManager.ChangeMusicVolume(1);
        gameObject.SetActive(false);
    }
}
