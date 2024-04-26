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
    [SerializeField] private List<Transform> powerUpsPrefab;
    [SerializeField] private List<Transform> powerUpSpawnLocations;
    [SerializeField] private float minTimeToSpawnPrefab = 55;
    [SerializeField] private float maxTimeToSpawnPrefab = 65;
    private float _timer = 0;
    private float _timeToSpawnPrefab = 60;
    private List<Transform> _powerUpSpawnLocationsTemp;
    private Transform _powerUpSpawnLocation;
    private bool _isNetworkSpawned = false;
    
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
        _powerUpSpawnLocationsTemp = new List<Transform>(powerUpSpawnLocations);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _isNetworkSpawned = true;
        GenerateRandomSpawnTimeAndResetTimerServerRpc();
    }

    private void Update()
    {
        if (!_isNetworkSpawned)
        {
            return;
        }
        if (_powerUpSpawnLocationsTemp.Count <= 0)
        {
            return;
        }
        _timer += Time.deltaTime;
        if (_timer >= _timeToSpawnPrefab)
        {
            SpawnPowerUpServerRpc();
            GenerateRandomSpawnTimeAndResetTimerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPowerUpServerRpc()
    {
        int prefabIndex = Random.Range(0, powerUpsPrefab.Count);
        Transform powerUpTransform = Instantiate(powerUpsPrefab[prefabIndex], _powerUpSpawnLocation.position, Quaternion.identity);
        NetworkObject powerUp = powerUpTransform.GetComponent<NetworkObject>();
        powerUp.Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GenerateRandomSpawnTimeAndResetTimerServerRpc()
    {
        _timeToSpawnPrefab = Random.Range(minTimeToSpawnPrefab, maxTimeToSpawnPrefab);
        Debug.Log(_timeToSpawnPrefab);
        
        if (_powerUpSpawnLocationsTemp.Count == 0)
        {
            return; // Return early to avoid accessing an out-of-range index
        }
        
        int powerUpSpawnLocationIndex = Random.Range(0, _powerUpSpawnLocationsTemp.Count);
        if (powerUpSpawnLocationIndex >= 0 && powerUpSpawnLocationIndex < _powerUpSpawnLocationsTemp.Count)
        {
            _powerUpSpawnLocation = _powerUpSpawnLocationsTemp[powerUpSpawnLocationIndex];
            _powerUpSpawnLocationsTemp.RemoveAt(powerUpSpawnLocationIndex);
            _timer = 0;
        }
    }
}
