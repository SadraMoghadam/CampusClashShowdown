using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RewardsUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI boxesDeliveredText;
    [SerializeField] private TextMeshProUGUI boxesDestroyedText;
    [SerializeField] private TextMeshProUGUI boxesPlacedText;
    [SerializeField] private TextMeshProUGUI beltMovementText;
    [SerializeField] private TextMeshProUGUI totalResourcesText;
    // [SerializeField] private TextMeshProUGUI team2Score;
    [SerializeField] private Button playAgainButton;


    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            NetworkLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            MultiplayerController.Instance.Restart();
            GameManager.LoadScene(GameManager.Scene.CampusScene);
        });

        CalculateRewards();
    }

    private void CalculateRewards()
    {
        PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromClientId(OwnerClientId);
        
        Team team = playerData.teamId == 1 ? Team.Team1 : Team.Team2;
        boxesDeliveredText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(team, RewardType.BoxDelivery).ToString();
        boxesDestroyedText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(team, RewardType.BoxDestruction).ToString();
        boxesPlacedText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(team, RewardType.BoxConveyorPlacement).ToString();
        beltMovementText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(team, RewardType.BeltMovement).ToString(); 
        int totalReward = ClashRewardCalculator.Instance.CalculateRewards(team);
        totalResourcesText.text = "+ " + totalReward;
    }

    public void Show() {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
