using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampusUI : MonoBehaviour
{
    [SerializeField] private Button clashButton;

    private void Awake()
    {
        clashButton.onClick.AddListener(() => 
            GameManager.LoadScene(GameManager.Scene.NetworkLobbyScene)
            );
    }
}
