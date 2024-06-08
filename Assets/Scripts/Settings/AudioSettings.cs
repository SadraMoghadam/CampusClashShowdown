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
        float masterVolume = PlayerPrefsManager.GetFloat(PlayerPrefsKeys.MasterVolume, 1);
        float musicVolume = PlayerPrefsManager.GetFloat(PlayerPrefsKeys.MusicVolume, 1);
        float sfxVolume = PlayerPrefsManager.GetFloat(PlayerPrefsKeys.SfxVolume, 1);
        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        effectsVolumeSlider.value = sfxVolume;
    }

    private void ChangeMasterVolume(float value)
    {
        float coefficient = value / ((masterVolumeSlider.maxValue - masterVolumeSlider.minValue));
        _audioManager.ChangeMasterVolume(coefficient);
        PlayerPrefsManager.SetFloat(PlayerPrefsKeys.MasterVolume, coefficient);
    }

    private void ChangeMusicVolume(float value)
    {
        float coefficient = value / ((musicVolumeSlider.maxValue - musicVolumeSlider.minValue));
        _audioManager.ChangeMusicVolume(coefficient);
        PlayerPrefsManager.SetFloat(PlayerPrefsKeys.MusicVolume, coefficient);
    }

    private void ChangeEffectsVolume(float value)
    {
        float coefficient = value / ((effectsVolumeSlider.maxValue - effectsVolumeSlider.minValue));
        _audioManager.ChangeEffectsVolume(coefficient);
        PlayerPrefsManager.SetFloat(PlayerPrefsKeys.SfxVolume, coefficient);
    }
}
