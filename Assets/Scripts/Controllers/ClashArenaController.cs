using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class can be used as a singleton and has the control info and functionalities that will be needed in
/// the ClashArena scenes 
/// </summary>
public class ClashArenaController : MonoBehaviour
{
    public Transform[] spawnLocations;
    
    private GameManager _gameManager;
    
    private static ClashArenaController _instance;
    public static ClashArenaController Instance => _instance;

    public enum ObjectType
    {
        Interactable, // able to press
        Pickable, // able to pick up and put down
        Pushable, // able to push or pull
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _gameManager = GameManager.Instance;
    }

    private void Start()
    {
        // _gameManager.AudioManager.play(SoundName.ClashArena);
    }
}
