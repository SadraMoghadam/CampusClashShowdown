using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PowerUpType
{
    Speed,
    Strength,
}
public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private float powerUpTime;
    [SerializeField] private float powerUpMultiplierCoefficient; 
    private PlayerController _playerController;
    private float _defaultPlayerSpeed;
    private float _defaultPlayerStrength;
    
    void Start()
    {
        _playerController = ClashArenaController.Instance.playerController;
        _defaultPlayerSpeed = _playerController.speed;
        _defaultPlayerStrength = _playerController.strength;
    }


    private void OnTriggerEnter(Collider other)
    {
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(PowerUpCR());
        
    }

    private IEnumerator PowerUpCR()
    {
        setPowerValue(powerUpMultiplierCoefficient);
        yield return new WaitForSeconds(powerUpTime);
        setPowerValue(1);
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private void setPowerValue(float coefficient)
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
}
