using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampusUI : MonoBehaviour
{
    [SerializeField] private Button clashButton;
    [SerializeField] private Button avatarButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private LeaderboardUI leaderBoardPanel;
    [SerializeField] private SettingsUI settingsPanel;
    
    
    [SerializeField] private TMP_Text playerName;

    private void Awake()
    {
        clashButton.onClick.AddListener(() =>
        {
            GameManager.LoadScene(GameManager.Scene.NetworkLobbyScene);
        });
        
        avatarButton.onClick.AddListener(() =>
        {
            CampusController.Instance.ChangeMode(toAvatarCustomizationMode: true);
        });
        
        leaderboardButton.onClick.AddListener(() =>
        {
            leaderBoardPanel.gameObject.SetActive(true);
        });
        
        settingsButton.onClick.AddListener(() =>
        {
            settingsPanel.gameObject.SetActive(true);
        });
    }

    private void OnEnable()
    {
        playerName.text = PlayerPrefsManager.GetString(PlayerPrefsKeys.PlayerName, "PlayerName");
        
    }
}
