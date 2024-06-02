using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    
    void Start()
    {
        fullScreenToggle.onValueChanged.AddListener(ToggleFullScreen);
        qualityDropdown.onValueChanged.AddListener(ChangeQuality);
    }

    private void ToggleFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }

    private void ChangeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, false);
    }
}
