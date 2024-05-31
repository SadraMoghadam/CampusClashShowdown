using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI team1Score;
    [SerializeField] private TextMeshProUGUI team2Score;
    [SerializeField] private Button rewardsButton;
    [SerializeField] private RewardsUI rewardsUI;
    [SerializeField] private Canvas mainCanvas;


    private void Awake() {
        rewardsButton.onClick.AddListener(() => {
            rewardsUI.gameObject.SetActive(true);
            Hide();
        });
    }

    private void Start() {
        ClashArenaController.Instance.OnStateChanged += ClashArenaController_OnStateChanged;
        Hide();
    }

    private void ClashArenaController_OnStateChanged(object sender, System.EventArgs e) {
        if (ClashArenaController.Instance.IsGameOver()) {
            mainCanvas.gameObject.SetActive(false);
            team1Score.text = MultiplayerController.Instance.GetTeamScore(1).ToString();
            team2Score.text = MultiplayerController.Instance.GetTeamScore(2).ToString();
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
        rewardsButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}