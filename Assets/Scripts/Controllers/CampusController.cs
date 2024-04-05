using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
