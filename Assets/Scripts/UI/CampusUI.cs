using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampusUI : MonoBehaviour
{
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private Button clashButton;
    [SerializeField] private Button avatarButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button settingsButton;
    
    [SerializeField] private LeaderboardUI leaderBoardPanel;
    [SerializeField] private SettingsUI settingsPanel;
    
    
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text resourcesValue;
    [SerializeField] private TMP_Text starValue;

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

        


        PlayerPrefsManager.SetString(PlayerPrefsKeys.ResourcesQty, "200" );

    }

   

    public void updateResources(int cost, int index, bool building)
    {
        if (building)
        {
            if (Int32.Parse(PlayerPrefsManager.GetString(PlayerPrefsKeys.ResourcesQty, "200")) - cost >= 0)
            {
                int updatedCost = Int32.Parse(PlayerPrefsManager.GetString(PlayerPrefsKeys.ResourcesQty, "200")) - cost;
                PlayerPrefsManager.SetString(PlayerPrefsKeys.ResourcesQty, updatedCost.ToString());
                resourcesValue.text = PlayerPrefsManager.GetString(PlayerPrefsKeys.ResourcesQty, "200");
            }
        }
        else
        {
            int updatedCost = Int32.Parse(PlayerPrefsManager.GetString(PlayerPrefsKeys.ResourcesQty, "200")) + cost;
            PlayerPrefsManager.SetString(PlayerPrefsKeys.ResourcesQty, updatedCost.ToString());
            resourcesValue.text = PlayerPrefsManager.GetString(PlayerPrefsKeys.ResourcesQty, "200");
        }
        
       
    }

    private void OnEnable()
    {
        playerName.text = PlayerPrefsManager.GetString(PlayerPrefsKeys.PlayerName, "PlayerName");
        starValue.text = PlayerPrefsManager.GetString(PlayerPrefsKeys.Stars, "200");
        resourcesValue.text = PlayerPrefsManager.GetString(PlayerPrefsKeys.ResourcesQty, "200");
        



    }
}
