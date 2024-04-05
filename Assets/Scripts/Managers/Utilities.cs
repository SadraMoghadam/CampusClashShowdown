using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Utilities : MonoBehaviour
{
    
    public static void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public static void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static IEnumerator FadeInAndOutCR(GameObject objectToFade, bool fadeIn, float duration, float finalOpacity = 1)
    {
        var instance = FindObjectOfType<Utilities>();
        instance.StopAllCoroutines();
        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0;
            b = finalOpacity;
        }
        else
        {
            a = finalOpacity;
            b = 0;
        }

        Color currentColor = Color.clear;

        SpriteRenderer tempSPRenderer = objectToFade.GetComponentInChildren<SpriteRenderer>();
        Image tempImage = objectToFade.GetComponentInChildren<Image>();
        RawImage tempRawImage = objectToFade.GetComponentInChildren<RawImage>();
        MeshRenderer tempRenderer = objectToFade.GetComponentInChildren<MeshRenderer>();
        TMP_Text tempText = objectToFade.GetComponentInChildren<TMP_Text>();

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            if (tempSPRenderer != null)
            {
                currentColor = tempSPRenderer.color;
                tempSPRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }
            
            if (tempImage != null)
            {
                currentColor = tempImage.color;
                tempImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }
            
            if (tempRawImage != null)
            {
                currentColor = tempRawImage.color;
                tempRawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }
            
            if (tempText != null)
            {
                currentColor = tempText.color;
                tempText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }
            
            if (tempRenderer != null)
            {
                currentColor = tempRenderer.material.color;
                tempRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }

            yield return null;
        }
        instance.StopCoroutine(FadeInAndOutCR(objectToFade, fadeIn, duration, finalOpacity));
    }
}
