using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RewardsUI : MonoBehaviour
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
            // NetworkLobby.Instance.LeaveLobby();
            // NetworkManager.Singleton.Shutdown();
            // MultiplayerController.Instance.Restart();
            GameManager.LoadScene(GameManager.Scene.CampusScene);
        });

        CalculateRewards();
    }

    private async void CalculateRewards()
    {
        boxesDeliveredText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(RewardType.BoxDelivery).ToString();
        boxesDestroyedText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(RewardType.BoxDestruction).ToString();
        boxesPlacedText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(RewardType.BoxConveyorPlacement).ToString();
        beltMovementText.text = "X " + ClashRewardCalculator.Instance.GetTotalByRewardType(RewardType.BeltMovement).ToString(); 
        int totalReward = await ClashRewardCalculator.Instance.CalculateRewards();
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
