using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    public Transform contentPanel;
    public GameObject entryPrefab;


    private void Start()
    {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void OnEnable()
    {
        GameManager.Instance.LeaderboardManager.FetchAndDisplayLeaderboard(this);
    }


    public void Show() 
    {
        gameObject.SetActive(true);
    }

    private void Hide() 
    {
        gameObject.SetActive(false);
    }
}
