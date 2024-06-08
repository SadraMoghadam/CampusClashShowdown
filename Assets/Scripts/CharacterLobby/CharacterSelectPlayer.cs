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
        if (playerIndex == 0)
        {
            kickButton.gameObject.SetActive(false);
        }

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
            Debug.Log(playerData);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            // Tuple<int, int> playerMeshIndices = PlayerPrefsManager.GetHeadAndBodyMeshIndices();
            playerVisual.SetPlayerMesh(MultiplayerController.Instance.GetPlayerHeadMesh(playerData.headMeshId),
                MultiplayerController.Instance.GetPlayerBodyMesh(playerData.bodyMeshId));
            playerNameText.text = playerData.playerName.ToString();
            SetCharacterTeamColor(playerIndex, playerData.clientId);
            SetCharacterNameColor(playerIndex);

        } else {
            Hide();
        }
    }

    private void SetCharacterTeamColor(int playerIndex, ulong clientId)
    {
        if (!CharacterSelectReady.Instance.IsPlayerReady(clientId))
        {
            return;
        }

        PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
        if (playerData.teamId == 1)
        {
            readyGameObject.GetComponent<TMP_Text>().color = GameManager.Instance.team1.color;
        }
        else
        {
            readyGameObject.GetComponent<TMP_Text>().color = GameManager.Instance.team2.color;
        }
    }

    private void SetCharacterNameColor(int playerIndex)
    {
        
        PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
        if (playerData.teamId == 1)
        {
            playerNameText.GetComponent<TMP_Text>().color = GameManager.Instance.team1.color;
        }
        else
        {
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