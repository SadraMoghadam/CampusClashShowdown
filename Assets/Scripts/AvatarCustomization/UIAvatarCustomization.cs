using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UIAvatarCustomization : MonoBehaviour
{
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button bodyPlusButton;
    [SerializeField] private Button bodyMinusButton;
    [SerializeField] private Button headPlusButton;
    [SerializeField] private Button headMinusButton;
    [SerializeField] private PlayerCustomization playerCustomization;
    [SerializeField] private TMP_InputField playerName;

    private string _playerInitialName;
    // Start is called before the first frame update
    private void Awake()
    {
        
        _playerInitialName = GetPlayerName();
        playerName.text = _playerInitialName;
        
        bodyPlusButton.onClick.AddListener(() =>
        {
            Debug.Log("pressed Button");
            playerCustomization.ChangeBodyPart(BodyPartType.Body, true);
            
            
        });  
        
        bodyMinusButton.onClick.AddListener(() =>
        {
            Debug.Log("pressed Button");
            playerCustomization.ChangeBodyPart(BodyPartType.Body, false);
        });  

        headPlusButton.onClick.AddListener(() =>
        {
            Debug.Log("pressed Head Button");
            playerCustomization.ChangeBodyPart(BodyPartType.Head, true);        
        });
        
        headMinusButton.onClick.AddListener(() =>
        {
            Debug.Log("pressed Head Button");
            playerCustomization.ChangeBodyPart(BodyPartType.Head, false);        
        });
        
        acceptButton.onClick.AddListener(() =>
        {
            // GameManager.Instance.AudioManager.ChangeMusicVolume(1);
            playerCustomization.SaveBodyPartData();
            SetPlayerName(playerName.text);
            CampusController.Instance.ChangeMode(false);
            PlayerPrefsManager.SetBool(PlayerPrefsKeys.GameStarted, true);
        });
        
        resetButton.onClick.AddListener(() =>
        {
            playerCustomization.ResetBodyPartData();
            // SetPlayerName(_playerInitialName);
        });
    }

    private void OnEnable()
    {
        // GameManager.Instance.AudioManager.ChangeMusicVolume(.6f);
    }

    private async void SetPlayerName(string name)
    {
        if (name != "")
        {
         
            PlayerPrefsManager.SetString(PlayerPrefsKeys.PlayerName, name);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(name);   
        }
    }
    
    
    private string GetPlayerName()
    {
        
        return PlayerPrefsManager.GetString(PlayerPrefsKeys.PlayerName, "");
    }

}
  