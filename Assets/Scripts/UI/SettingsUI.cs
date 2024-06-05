using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    
    [SerializeField] private Button closeButton;
    [SerializeField] private Button quitButton;
    
    private GameManager _gameManager;

    private void Awake()
    {
        
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
        
        quitButton.onClick.AddListener(() =>
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
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
