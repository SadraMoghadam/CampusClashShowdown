using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start() {
        MultiplayerController.Instance.OnTryingToJoinGame += MultiplayerController_OnTryingToJoinGame;
        MultiplayerController.Instance.OnFailedToJoinGame += MultiplayerController_OnFailedToJoinGame;

        Hide();
    }

    private void MultiplayerController_OnFailedToJoinGame(object sender, System.EventArgs e) {
        Hide();
    }

    private void MultiplayerController_OnTryingToJoinGame(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerController.Instance.OnTryingToJoinGame -= MultiplayerController_OnTryingToJoinGame;
        MultiplayerController.Instance.OnFailedToJoinGame -= MultiplayerController_OnFailedToJoinGame;
    }

}
