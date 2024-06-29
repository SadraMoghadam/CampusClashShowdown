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
    [SerializeField] private Button openPanelButton;
    [SerializeField] private Button closePanelButton;
    [SerializeField] private Animator panelAnimator;
    
    [SerializeField] private LeaderboardUI leaderBoardPanel;
    [SerializeField] private SettingsUI settingsPanel;
    
    
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text resourceValue;
    [SerializeField] private TMP_Text starValue;

    private void Awake()
    {
        GameManager.Instance.AddButtonsSound();
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
        
        openPanelButton.onClick.AddListener(() =>
        {
            panelAnimator.SetTrigger("Open");
        });

        closePanelButton.onClick.AddListener(() =>
        {
            panelAnimator.SetTrigger("Close");
        });

        //PlayerPrefsManager.SetString(PlayerPrefsKeys.Resource, "200" );

    }

   

    public void updateResources(int cost, int index, bool building)
    {
        int resources = PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, GameManager.InitialResources);
        if (building)
        {
            if (resources - cost >= 0)
            {
                int updatedCost = resources - cost;
                PlayerPrefsManager.SetInt(PlayerPrefsKeys.Resource, updatedCost);
                resourceValue.text = updatedCost.ToString();
            }
        }
        else
        {
            int updatedCost = resources + cost;
            PlayerPrefsManager.SetInt(PlayerPrefsKeys.Resource, updatedCost);
            resourceValue.text = updatedCost.ToString();
        }
        
       
    }

    private void OnEnable()
    {
        playerName.text = PlayerPrefsManager.GetString(PlayerPrefsKeys.PlayerName, "PlayerName");
        resourceValue.text = PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, GameManager.InitialResources).ToString();
        starValue.text = PlayerPrefsManager.GetInt(PlayerPrefsKeys.Stars, 200).ToString();


    }
}
