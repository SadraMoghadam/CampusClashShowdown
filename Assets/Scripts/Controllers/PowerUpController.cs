using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpController : NetworkBehaviour
{
    [SerializeField] private List<Transform> powerUpsPrefab;
    [SerializeField] private List<Transform> powerUpSpawnLocations;
    [SerializeField] private float minTimeToSpawnPrefab = 55;
    [SerializeField] private float maxTimeToSpawnPrefab = 65;
    [SerializeField] private int maxNumOfPowerUps = 3;
    private float _timer = 0;
    private float _timeToSpawnPrefab = 60;
    private List<Transform> _powerUpSpawnLocationsTemp;
    private Transform _powerUpSpawnLocation;
    private bool _isNetworkSpawned = false;
    private int _numOfPowerUps = 0;
    
    private void Awake() {
        _powerUpSpawnLocationsTemp = new List<Transform>(powerUpSpawnLocations);
    }
    
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _isNetworkSpawned = true;
        _numOfPowerUps = 0;
        
        ClashArenaController.Instance.OnStateChanged += ClashArenaController_OnStateChanged;
    }
    
    
    private void ClashArenaController_OnStateChanged(object sender, System.EventArgs e)
    {
        if (ClashArenaController.Instance.IsGamePlaying())
        {
            GenerateRandomSpawnTimeAndResetTimerServerRpc();
        }
    }

    private void Update()
    {
        if (!_isNetworkSpawned)
        {
            return;
        }
        if (_powerUpSpawnLocationsTemp.Count <= 0 || maxNumOfPowerUps == _numOfPowerUps)
        {
            return;
        }
        _timer += Time.deltaTime;
        if (_timer >= _timeToSpawnPrefab)
        {
            SpawnPowerUpServerRpc();
            GenerateRandomSpawnTimeAndResetTimerServerRpc();
            _timer = 0;
            _numOfPowerUps++;
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
        }
    }
}
