using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The name of all of the sounds, that are used in the game, is defined in SoundName enumerator 
/// </summary>
public enum SoundName
{
    CampusTheme,
    ClashTheme,
    Sprint,
    Dragging,
    PressingButton,
    PickBox,
    DropBox,
    DestroyedBox,
    BoxDelivered,
    PowerUpGained,
    PowerUpFinished,
    GameOver,
    GatesMovement,
    Button1,
    Button2,
}

/// <summary>
/// Sound class is used in the inspector to get the info of each sound (music or SFX). 
/// </summary>
[System.Serializable]
public class Sound
{
    public SoundName name;
    public AudioClip clip;
    [Range(0f, 1.5f)] public float volume;
    [Range(.1f, 3f)] public float pitch;
    public bool loop;
    public AudioSource source;
}

/// <summary>
/// This class can be used through GameManager instance and there is every function that can be used to play
/// or change the volume of a sound. The functions work only by giving the name of the sound that we
/// want to play, as input.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // for (int i = 0; i < sounds.Length; i++)
        // {
        //     // sounds[i].source = gameObject.GetComponent<AudioSource>();
        //     // if(sounds[i].source == null)
        //     //     gameObject.AddComponent<AudioSource>();
        //     if(sounds[i].source == null)
        //         continue;
        //     sounds[i].source.clip = sounds[i].clip;
        //     sounds[i].source.volume = sounds[i].volume;
        //     sounds[i].source.pitch = sounds[i].pitch;
        //     sounds[i].source.loop = sounds[i].loop;
        // }
    }

    public void ChangeMasterVolume(float coefficient)
    {
        PlayerPrefsManager.SetFloat(PlayerPrefsKeys.MasterVolume, coefficient);
        AudioListener.volume = coefficient;
    }

    public void ChangeMusicVolume(float coefficient)
    {
        PlayerPrefsManager.SetFloat(PlayerPrefsKeys.MusicVolume, coefficient);
        for (int i = 0; i < 2; i++)
        {
            if(sounds[i].source == null)
                continue;
            sounds[i].source.volume = coefficient;
        }
    }

    public void ChangeEffectsVolume(float coefficient)
    {
        PlayerPrefsManager.SetFloat(PlayerPrefsKeys.SfxVolume, coefficient);
        for (int i = 2; i < sounds.Length; i++)
        {
            sounds[i].volume = coefficient;
        }
    }
    
    public void Play(SoundName name, bool isGameManager = false)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            return;
        }

        try
        {
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.Play();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void Stop(SoundName name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            return;
        }

        try
        {
            sound.source.Stop();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    public void Instantplay(SoundName name, Vector3 position)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            return;
        }

        try
        {
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position;

            AudioSource audioSource = tempGO.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
            audioSource.loop = sound.loop;
            audioSource.Play();

            if (!sound.loop)
            {
                Destroy(tempGO, sound.clip.length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}