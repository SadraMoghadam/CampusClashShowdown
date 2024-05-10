using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button playAgainButton;


    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
        });
    }

    private void Start() {
        ClashArenaController.Instance.OnStateChanged += ClashArenaController_OnStateChanged;

        Hide();
    }

    private void ClashArenaController_OnStateChanged(object sender, System.EventArgs e) {
        if (ClashArenaController.Instance.IsGameOver()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}