using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip introVideo;
    [SerializeField] private VideoClip logoVideo;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("GameStarted"))
        {
            videoPlayer.clip = logoVideo;
        }
        else
        {
            videoPlayer.clip = introVideo;
        }
        videoPlayer.Play();
        
        videoPlayer.loopPointReached += CheckOver;
    }
    
    
    void CheckOver(VideoPlayer vp)
    {
        SceneManager.LoadScene("CampusScene");
    }
}
