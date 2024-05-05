using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider pushOrPullTimerSlider;
    private Transform _mainCameraTransform;
    
    
    // public static PlayerUI Instance => _instance;
    // private static PlayerUI _instance;
    

    void Awake()
    {
        // if (_instance == null)
        // {
        //     _instance = this;
        // }
        pushOrPullTimerSlider.gameObject.SetActive(false);
        _mainCameraTransform = Camera.main.transform;
        UpdateUIPosition();
    }

    void LateUpdate()
    {
        // Update UI position every frame to face the camera
        UpdateUIPosition();
    }

    void UpdateUIPosition()
    {
        // Face the UI element towards the camera
        transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward,
            _mainCameraTransform.rotation * Vector3.up);
    }

    public void SetPushOrPullTimer(float value)
    {
        pushOrPullTimerSlider.gameObject.SetActive(true);
        pushOrPullTimerSlider.value = value;
    }
    
    public void DisablePushOrPullTimerSlider()
    {
        pushOrPullTimerSlider.gameObject.SetActive(false);
    }
}
