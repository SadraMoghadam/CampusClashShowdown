using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {


    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;
    

    private void Awake() {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            NetworkLobby.Instance.KickPlayer(playerData.playerId.ToString());
            MultiplayerController.Instance.KickPlayer(playerData.clientId);
        });
    }
    
    private void Start() {
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged += MultiplayerController_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        // StartCoroutine(UpdatePlayerCR());
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void MultiplayerController_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        // StartCoroutine(UpdatePlayerCR());
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (MultiplayerController.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();

            PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            // Tuple<int, int> playerMeshIndices = PlayerPrefsManager.GetHeadAndBodyMeshIndices();
            playerVisual.SetPlayerMesh(MultiplayerController.Instance.GetPlayerHeadMesh(playerData.headMeshId),
                MultiplayerController.Instance.GetPlayerBodyMesh(playerData.bodyMeshId));
            playerNameText.text = playerData.playerName.ToString();
            SetCharacterTeamColor(playerIndex, playerData.clientId);

        } else {
            Hide();
        }
    }

    private void SetCharacterTeamColor(int playerIndex, ulong clientId)
    {
        if (!CharacterSelectReady.Instance.IsPlayerReady(clientId))
        {
            if (playerIndex < 2)
            {
                playerNameText.GetComponent<TMP_Text>().color = GameManager.Instance.team1.color;
            }
            else
            {
                playerNameText.GetComponent<TMP_Text>().color = GameManager.Instance.team2.color;
            }
            return;
        }
        if (playerIndex < 2)
        {
            readyGameObject.GetComponent<TMP_Text>().color = GameManager.Instance.team1.color;
            playerNameText.GetComponent<TMP_Text>().color = GameManager.Instance.team1.color;
        }
        else
        {
            readyGameObject.GetComponent<TMP_Text>().color = GameManager.Instance.team2.color;
            playerNameText.GetComponent<TMP_Text>().color = GameManager.Instance.team2.color;
        }
    }

    private IEnumerator UpdatePlayerCR()
    {
        yield return new WaitForSeconds(.2f);
        UpdatePlayer();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged -= MultiplayerController_OnPlayerDataNetworkListChanged;
    }


}