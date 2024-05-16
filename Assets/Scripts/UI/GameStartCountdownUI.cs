using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour 
{
    private const string NUMBER_POPUP = "NumberPopup";


    [SerializeField] private TextMeshProUGUI countdownText;


    private Animator animator;
    private int previousCountdownNumber;

    
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        ClashArenaController.Instance.OnStateChanged += ClashArenaController_OnStateChanged;

        Hide();
    }

    private void ClashArenaController_OnStateChanged(object sender, System.EventArgs e) {
        if (ClashArenaController.Instance.IsCountdownToStartActive()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        int countdownNumber = Mathf.CeilToInt(ClashArenaController.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();

        if (previousCountdownNumber != countdownNumber) {
            previousCountdownNumber = countdownNumber;
            animator.SetTrigger(NUMBER_POPUP);
            // GameManager.Instance.AudioManager.play();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}