using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public enum PowerUpType
{
    Speed,
    Strength,
}
public class PowerUp : NetworkBehaviour
{
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private float powerUpTime;
    [SerializeField] private float powerUpMultiplierCoefficient; 
    private PlayerController _playerController;
    private float _defaultPlayerSpeed;
    private float _defaultPlayerStrength;
    private ClashSceneUI _clashSceneUI;
    
    void Start()
    {
        // StartCoroutine(WaitForPlayerControllerInitialization());
        _clashSceneUI = ClashSceneUI.Instance;
    }
    
    // private IEnumerator WaitForPlayerControllerInitialization()
    // {
    //     // Wait until the PlayerController instance is available
    //     while (PlayerController.Instance == null)
    //     {
    //         yield return null;
    //     }
    //     _defaultPlayerSpeed = _playerController.speed;
    //     _defaultPlayerStrength = _playerController.strength;
    // }


    private void OnTriggerEnter(Collider other)
    {
        SetUI();
        _playerController = other.GetComponent<PlayerController>();
        SetDefaultPlayerAttributes();
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(PowerUpCR());

    }

    private IEnumerator PowerUpCR()
    {
        SetPowerValue(powerUpMultiplierCoefficient);
        yield return new WaitForSeconds(powerUpTime);
        SetPowerValue(1);
        StopAllCoroutines();
        DestroyPowerUpServerRpc();
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    private void DestroyPowerUpServerRpc() 
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    private void SetPowerValue(float coefficient)
    {
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                _playerController.speed = _defaultPlayerSpeed * coefficient;
                break;
            case PowerUpType.Strength:
                _playerController.strength = _defaultPlayerStrength * coefficient;
                break;
            default:
                _playerController.speed = _defaultPlayerSpeed;
                _playerController.strength = _defaultPlayerStrength;
                break;
        }
    }

    private void SetDefaultPlayerAttributes()
    {
        _defaultPlayerSpeed = _playerController.speed;
        _defaultPlayerStrength = _playerController.strength;
    }

    private void SetUI()
    {
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                _clashSceneUI.SetSpeedPowerUpSliderValue(powerUpTime);
                break;
            case PowerUpType.Strength:
                _clashSceneUI.SetStrengthPowerUpSliderValue(powerUpTime);
                break;
            default:
                break;
        }
    }
}
