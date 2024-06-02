using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    
    [SerializeField] private Button closeButton;
    
    private GameManager _gameManager;

    private void Awake()
    {
        
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void OnEnable()
    {
        // _gameManager.AudioManager.play(SoundName.SettingsMenu);
        
    }
    //
    // private void OnGraphicsClicked()
    // {
    //     
    // }
    //
    // private void OnAudioClicked()
    // {
    //     
    // }
    //
    // private void OnControlsClicked()
    // {
    //     
    // }
    //
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
