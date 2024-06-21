using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text staminaText;
    [SerializeField] private Image background;
    [SerializeField] private Image fillRect;
    private float _fadeTime = 1f;

    private void Start()
    {
        slider.value = 100;
        staminaText.text = ((int)100).ToString();
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
        fillRect.color = new Color(fillRect.color.r, fillRect.color.g, fillRect.color.b, 0);
    }

    public void FadeIn()
    {
        if (background.color.a == 0)
        {
            StartCoroutine(FadeInProcess());
        }
    }

    public void FadeOut()
    {
        if (background.color.a == 1)
        {
            StartCoroutine(FadeOutProcess());
        }
    }
    
    private IEnumerator FadeInProcess() 
    {
        Color backgroundColor = background.color;
        Color fillRectColor = fillRect.color;
        while(Mathf.Abs(backgroundColor.a - 1) > 0.1f) 
        {
            backgroundColor.a = Mathf.Lerp(backgroundColor.a, 1, _fadeTime * Time.deltaTime);
            background.color = backgroundColor;
            fillRectColor.a = Mathf.Lerp(fillRectColor.a, 1, _fadeTime * Time.deltaTime);
            fillRect.color = fillRectColor;
            yield return null;
        }
        backgroundColor.a = 1;
        fillRectColor.a = 1;
        background.color = backgroundColor;
        fillRect.color = fillRectColor;
    }

    private IEnumerator FadeOutProcess()
    {
        Color backgroundColor = background.color;
        Color fillRectColor = fillRect.color;
        while(Mathf.Abs(backgroundColor.a - 0) > 0.1f) 
        {
            backgroundColor.a = Mathf.Lerp(backgroundColor.a, 0, _fadeTime * Time.deltaTime);
            background.color = backgroundColor;
            fillRectColor.a = Mathf.Lerp(fillRectColor.a, 0, _fadeTime * Time.deltaTime);
            fillRect.color = fillRectColor;
            yield return null;
        }

        backgroundColor.a = 0;
        fillRectColor.a = 0;
        background.color = backgroundColor;
        fillRect.color = fillRectColor;
    }
    
}
