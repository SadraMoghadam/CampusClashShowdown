using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AvatarEndGame : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private TextMeshPro playerNameText;
    

    
    private void Start() {
        MultiplayerController.Instance.OnPlayerDataNetworkListChanged += MultiplayerController_OnPlayerDataNetworkListChanged;
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
            playerVisual.SetPlayerMesh(MultiplayerController.Instance.GetPlayerHeadMesh(playerData.headMeshId),
                MultiplayerController.Instance.GetPlayerBodyMesh(playerData.bodyMeshId));
            playerNameText.text = playerData.playerName.ToString();
            SetCharacterNameColor(playerIndex);

        } 
    }

    private void SetCharacterNameColor(int playerIndex)
    {
        if (playerIndex < 2)
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
        try
        {
            MultiplayerController.Instance.OnPlayerDataNetworkListChanged -= MultiplayerController_OnPlayerDataNetworkListChanged;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
