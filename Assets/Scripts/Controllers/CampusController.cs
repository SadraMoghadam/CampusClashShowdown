using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This Class can be used as a singleton and has the control info and functionalities that will be needed in
/// the Campus scene 
/// </summary>
public class CampusController : MonoBehaviour
{
    private GameManager _gameManager;
    
    private static CampusController _instance;
    public static CampusController Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        Time.timeScale = 1;
        _gameManager = GameManager.Instance;
    }

    private void Start()
    {
        bool isGameStarted = PlayerPrefsManager.GetBool(PlayerPrefsKeys.GameStarted, false);
        if (!isGameStarted)
        {
            PlayerPrefsManager.SetBool(PlayerPrefsKeys.GameStarted, true);
        }
        // _gameManager.AudioManager.play(SoundName.CampusArea);
        // DialogueController.Show(1);
    }

}