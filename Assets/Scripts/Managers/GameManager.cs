using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// GameManager manages everything in the game and is alive between scenes. Also it should not be destroyed after the
/// change of scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    [NonSerialized] public AudioManager AudioManager;
    public static GameManager Instance => _instance;
    private static GameManager _instance;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        _instance = this;
        
        GameManager[] gameManagers = FindObjectsOfType<GameManager>();
        if(gameManagers.Length > 1)
        {
            for (int i = 0; i < gameManagers.Length - 1; i++)
            {
                Destroy(gameManagers[i].gameObject);
            }
        }
        
        // dont destroy the gameObject, that GameManager is attached to, after scene change 
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// For each scene change we can call this function
    /// </summary>
    /// <param name="sceneName"></param>
    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        SceneManager.LoadScene($"Loading");
        scene.allowSceneActivation = false;
        await Task.Delay(400);
        var slider = FindObjectOfType<Slider>();
        do
        {
            await Task.Delay(500);
            slider.value = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);
        scene.allowSceneActivation = true;
        SceneManager.LoadScene(sceneName);
    }
}
