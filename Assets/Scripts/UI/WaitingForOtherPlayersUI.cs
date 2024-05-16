using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    private ClashArenaController _clashArenaController;

    private void Start()
    {
        _clashArenaController = ClashArenaController.Instance;
        _clashArenaController.OnLocalPlayerReadyChanged += ClashArenaController_OnLocalPlayerReadyChanged;
        _clashArenaController.OnStateChanged += ClashArenaController_OnStateChanged;

        Hide();
    }

    private void ClashArenaController_OnStateChanged(object sender, System.EventArgs e) {
        if (_clashArenaController.IsCountdownToStartActive()) {
            Hide();
        }
    }

    private void ClashArenaController_OnLocalPlayerReadyChanged(object sender, System.EventArgs e) {
        if (_clashArenaController.IsLocalPlayerReady()) {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}