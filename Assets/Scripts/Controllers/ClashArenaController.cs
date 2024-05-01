using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This Class can be used as a singleton and has the control info and functionalities that will be needed in
/// the ClashArena scenes 
/// </summary>
public class ClashArenaController : NetworkBehaviour
{
    public List<Transform> spawnLocations;
    
    private GameManager _gameManager;
    
    private static ClashArenaController _instance;
    public static ClashArenaController Instance => _instance;

    public TeamCharacteristicsScriptableObject team1;
    public TeamCharacteristicsScriptableObject team2;

    public enum ObjectType
    {
        Pressable, // able to press
        Pickable, // able to pick up and put down
        Pushable, // able to push or pull
    }
    
    // private enum State {
    //     WaitingToStart,
    //     CountdownToStart,
    //     GamePlaying,
    //     GameOver,
    // }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _gameManager = GameManager.Instance;
    }

}
