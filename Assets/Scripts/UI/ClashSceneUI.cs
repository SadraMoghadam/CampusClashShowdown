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
    [SerializeField] private Image team1Background;
    [SerializeField] private Image team2Background;

    private ClashArenaController _clashArenaController;

    public static ClashSceneUI Instance => _instance;
    private static ClashSceneUI _instance;

    private Coroutine _conveyorStopCooldownCR;
    private Coroutine _speedPowerUpCR;
    private Coroutine _strengthPowerUpCR;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _clashArenaController = ClashArenaController.Instance;
        team1Background.color = _clashArenaController.team1.color;
        team2Background.color = _clashArenaController.team2.color;
        team1Score.text = "0";
        team2Score.text = "0";
        conveyorStopCooldown.gameObject.SetActive(false);
        speedPowerUp.gameObject.SetActive(false);
        strengthPowerUp.gameObject.SetActive(false);
        
    }

    public void SetScore(int teamId, int score)
    {
        if (teamId == 1)
        {
            team1Score.text = score.ToString();
        }
        else if (teamId == 2)
        {
            team2Score.text = score.ToString();
        }
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

        while (timer < fillTime)
        {
            timer += Time.deltaTime;
            
            float currentFillAmount = Mathf.Lerp(0f, 1, timer / fillTime);
            slider.value = currentFillAmount;

            yield return null;
        }

        slider.value = 1;
        
        slider.gameObject.SetActive(false);
        
        if(slider.name == conveyorStopCooldown.name)
            StopCoroutine(_conveyorStopCooldownCR);
        else if(slider.name == speedPowerUp.name)
            StopCoroutine(_speedPowerUpCR);
        else if(slider.name == strengthPowerUp.name)
            StopCoroutine(_strengthPowerUpCR);
    }
}
