using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All the functions that can help the game development and are general
/// </summary>
public class Utilities : MonoBehaviour
{
    
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
    
    
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

    /// <summary>
    /// Used to fade in or out an UI element on Canvas using coroutine 
    /// </summary>
    /// <param name="objectToFade"></param>
    /// <param name="fadeIn"></param>
    /// <param name="duration"></param>
    /// <param name="finalOpacity"></param>
    /// <returns></returns>
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
