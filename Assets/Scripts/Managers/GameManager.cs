using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// GameManager manages everything in the game and is alive between scenes. Also it should not be destroyed after the
/// change of scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    
    public enum Scene {
        CampusScene,
        ClashScene1,
        CharactersLobbyScene,
        NetworkLobbyScene,
        LoadingScene,
    }

    [NonSerialized] public AudioManager AudioManager;
    public BodyPartData[] avatarBodyPartDataArray;
    public TeamCharacteristicsScriptableObject team1;
    public TeamCharacteristicsScriptableObject team2;
    public PlayerVisualScriptableObject playerMeshes;
    public static GameManager Instance => _instance;
    private static GameManager _instance;
    private static Scene _targetScene;
    
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
    public static async void LoadScene(Scene sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName.ToString());
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        scene.allowSceneActivation = false;
        await Task.Delay(200);
        var slider = FindObjectOfType<Slider>();
        slider.value = 0;
        do
        {
            await Task.Delay(300);
            slider.value = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(500);
        scene.allowSceneActivation = true;
        SceneManager.LoadScene(sceneName.ToString());
    }
    
    public static async void LoadSceneNetwork(Scene sceneName)
    {
        // var scene = SceneManager.LoadSceneAsync(sceneName.ToString());
        
        var scene = NetworkManager.Singleton.SceneManager.LoadScene(Scene.LoadingScene.ToString(), LoadSceneMode.Single);
        if (scene == SceneEventProgressStatus.Started)
        {
            await Task.Delay(100);
            var slider = FindObjectOfType<Slider>();
            slider.value = 0;
            do
            {
                await Task.Delay(100);
                slider.value += 0.1f;
            } while (slider.value <= 0.9f);
            slider.value += 0.1f; 
            await Task.Delay(300);
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);    
        }
    }
    
    
    public static void LoadNetwork(Scene targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
}
