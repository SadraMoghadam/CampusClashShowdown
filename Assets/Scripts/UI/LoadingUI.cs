using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{

    private void OnEnable()
    {
        GameManager.Instance.AudioManager.ChangeMusicVolume(.3f);
    }

    private void OnDestroy()
    {
        
        GameManager.Instance.AudioManager.ChangeMusicVolume(1);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
