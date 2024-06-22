using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClashSceneUI : MonoBehaviour
{
    [SerializeField] private TMP_Text team1Score;
    [SerializeField] private TMP_Text team2Score;
    [SerializeField] private Slider conveyorStopCooldown;
    [SerializeField] private Slider speedPowerUp;
    [SerializeField] private Slider strengthPowerUp;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] public CanvasGroup staminaCanvasGroup;
    [SerializeField] private Image team1Background;
    [SerializeField] private Image team2Background;
    [SerializeField] private Image timerImage;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button settingsButton;
    [SerializeField] private SettingsClashUI settingsClashUI;
    

    private ClashArenaController _clashArenaController;

    public static ClashSceneUI Instance => _instance;
    private static ClashSceneUI _instance;

    private Coroutine _conveyorStopCooldownCR;
    private Coroutine _speedPowerUpCR;
    private Coroutine _strengthPowerUpCR;
    private float maxStamina = 100;
    [HideInInspector] public bool isAbleToPress;

    private void Awake()
    {
        GameManager.Instance.AddButtonsSound();
        if (_instance == null)
        {
            _instance = this;
        }
        team1Background.color = GameManager.Instance.team1.color;
        team2Background.color = GameManager.Instance.team2.color;
        team1Score.text = "0";
        team2Score.text = "0";
        conveyorStopCooldown.gameObject.SetActive(false);
        speedPowerUp.gameObject.SetActive(false);
        strengthPowerUp.gameObject.SetActive(false);
        isAbleToPress = true;
        settingsButton.onClick.AddListener(() =>
        {
            settingsClashUI.gameObject.SetActive(true);
        });
        
        staminaCanvasGroup.alpha = 0;
    }

    private void Update() {
        timerImage.fillAmount = ClashArenaController.Instance.GetGamePlayingTimerNormalized();
        timerText.text = ClashArenaController.Instance.GetRemainingTime().ToString();
    }
    
    public void SetScore(Team team, int score)
    {
        if (team == Team.Team1)
        {
            team1Background.gameObject.GetComponent<Animator>().Play("TeamScore");
            team1Score.text = score.ToString();
        }
        else if (team == Team.Team2)
        {
            team2Background.gameObject.GetComponent<Animator>().Play("TeamScore");
            team2Score.text = score.ToString();
        }
    }
    
    
    public void SetStaminaSliderValue(float value)
    {
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = value;
    }
    
    public IEnumerator FadeStaminaBar(float targetAlpha)
    {
        float fadeDuration = 0.5f;
        float startAlpha = staminaCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            staminaCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        staminaCanvasGroup.alpha = targetAlpha;
    }

    public void SetConveyorStopCooldownSliderValue(float fillTime)
    {
        _conveyorStopCooldownCR = StartCoroutine(FillSliderOverTime(conveyorStopCooldown, fillTime));
    }
    
    public void SetSpeedPowerUpSliderValue(float fillTime)
    {
        _speedPowerUpCR = StartCoroutine(FillSliderOverTime(speedPowerUp, fillTime));
    }
    
    public void SetStrengthPowerUpSliderValue(float fillTime)
    {
        _strengthPowerUpCR = StartCoroutine(FillSliderOverTime(strengthPowerUp, fillTime));
    }
    
    private IEnumerator FillSliderOverTime(Slider slider, float fillTime)
    {
        slider.gameObject.SetActive(true);
        float timer = 0f;

        if (slider.name == conveyorStopCooldown.name)
        {
            isAbleToPress = false;
        }
        while (timer < fillTime)
        {
            timer += Time.deltaTime;
            
            float currentFillAmount = Mathf.Lerp(0f, 1, timer / fillTime);
            slider.value = currentFillAmount;

            yield return null;
        }

        slider.value = 1;
        
        slider.gameObject.SetActive(false);

        if (slider.name == conveyorStopCooldown.name)
        {
            isAbleToPress = true;
            StopCoroutine(_conveyorStopCooldownCR);   
        }
        else if(slider.name == speedPowerUp.name)
            StopCoroutine(_speedPowerUpCR);
        else if(slider.name == strengthPowerUp.name)
            StopCoroutine(_strengthPowerUpCR);
    }
    
    
}
