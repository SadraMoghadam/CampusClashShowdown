using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum KeyBindingType
{
    PickableArea,
    DeliveryArea,
    PushAndPullArea,
    BlockButtonArea,
    ConveyorButtonArea,
    DropBox
}

public class KeyBindingUI : MonoBehaviour
{
    public int id = 0;
    public KeyBindingType type;
    [SerializeField] private List<GameObject> keyBindingImagesGO;
    private Transform _mainCameraTransform;

    private void Awake()
    {
        _mainCameraTransform = Camera.main.transform;
        Hide();
    }

    private void Start()
    {
        transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward,
            _mainCameraTransform.rotation * Vector3.up);
    }

    public void Show()
    {
        keyBindingImagesGO.ForEach(imageGO => imageGO.SetActive(true));
    }
    
    public void Hide()
    {
        keyBindingImagesGO.ForEach(imageGO => imageGO.SetActive(false));
    }
}
