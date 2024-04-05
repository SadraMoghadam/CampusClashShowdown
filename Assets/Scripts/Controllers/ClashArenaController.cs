using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashArenaController : MonoBehaviour
{
    private GameManager _gameManager;
    
    private static ClashArenaController _instance;
    public static ClashArenaController Instance => _instance;

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
        // _gameManager.AudioManager.play(SoundName.ClashArena);
    }
}
