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
        try
        {

            _playerController = other.GetComponent<PlayerController>();
            SetUI();
            SetVFX();
            SetDefaultPlayerAttributes();
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(PowerUpCR());

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private IEnumerator PowerUpCR()
    {
        SetPowerValue(powerUpMultiplierCoefficient);
        yield return new WaitForSeconds(powerUpTime);
        if(_playerController.IsLocalPlayer)
            GameManager.Instance.AudioManager.Instantplay(SoundName.PowerUpFinished, transform.position);
        SetPowerValue(1);
        StopAllCoroutines();
        DestroyPowerUpServerRpc();
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    private void DestroyPowerUpServerRpc() 
    {
        try
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("didnt destroyed");
        }
    }

    private void SetPowerValue(float coefficient)
    {
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                _playerController.speed = _defaultPlayerSpeed * coefficient;
                _playerController.runSpeed = (_defaultPlayerSpeed + 1) * coefficient;
                break;
            case PowerUpType.Strength:
                _playerController.strength = _defaultPlayerStrength * coefficient;
                break;
            default:
                _playerController.speed = _defaultPlayerSpeed;
                _playerController.runSpeed = _defaultPlayerSpeed + 1;
                _playerController.strength = _defaultPlayerStrength;
                break;
        }
    }
    
    private void SetVFX()
    {
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                ClashVFXContainer.InstantiateVFX(ClashVFXType.SpeedPowerUp, Vector3.zero, _playerController.gameObject.transform, powerUpTime);
                break;
            case PowerUpType.Strength:
                ClashVFXContainer.InstantiateVFX(ClashVFXType.StrengthPowerUp, Vector3.zero, _playerController.gameObject.transform, powerUpTime);
                break;
            default:
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
        if (!_playerController.IsLocalPlayer)
            return;
        GameManager.Instance.AudioManager.Instantplay(SoundName.PowerUpGained, transform.position);
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
