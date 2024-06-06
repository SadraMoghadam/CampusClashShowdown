using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    
    [NonSerialized] private AudioManager _audioManager;

    void Start()
    {
        _audioManager = GameManager.Instance.AudioManager;
        masterVolumeSlider.onValueChanged.AddListener(ChangeMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
        effectsVolumeSlider.onValueChanged.AddListener(ChangeEffectsVolume);
    }

    private void ChangeMasterVolume(float value)
    {
        _audioManager.ChangeMasterVolume(value / ((masterVolumeSlider.maxValue - masterVolumeSlider.minValue)));
    }

    private void ChangeMusicVolume(float value)
    {
        _audioManager.ChangeMusicVolume(value / ((musicVolumeSlider.maxValue - musicVolumeSlider.minValue)));
    }

    private void ChangeEffectsVolume(float value)
    {
        _audioManager.ChangeEffectsVolume(value / ((effectsVolumeSlider.maxValue - effectsVolumeSlider.minValue)));
    }
}
