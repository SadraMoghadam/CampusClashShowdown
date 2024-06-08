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
    [NonSerialized] public LeaderboardManager LeaderboardManager;
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
        _instance = this;
        
        GameManager[] gameManagers = FindObjectsOfType<GameManager>();
        AudioManager = GetComponent<AudioManager>();
        LeaderboardManager = GetComponent<LeaderboardManager>();
        if(gameManagers.Length > 1)
        {
            for (int i = 0; i < gameManagers.Length - 1; i++)
            {
                Destroy(gameManagers[i].gameObject);
            }
        }
        
        // dont destroy the gameObject, that GameManager is attached to, after scene change 
        DontDestroyOnLoad(this.gameObject);
        SetSoundVolume();
    }

    private void SetSoundVolume()
    {
        float masterVolume = PlayerPrefsManager.GetFloat(PlayerPrefsKeys.MasterVolume, 1);
        float musicVolume = PlayerPrefsManager.GetFloat(PlayerPrefsKeys.MusicVolume, 1);
        float sfxVolume = PlayerPrefsManager.GetFloat(PlayerPrefsKeys.SfxVolume, 1);
        
        AudioManager.ChangeMasterVolume(masterVolume);
        AudioManager.ChangeMusicVolume(musicVolume);
        AudioManager.ChangeEffectsVolume(sfxVolume);
    }

    /// <summary>
    /// For each scene change we can call this function
    /// </summary>
    /// <param name="sceneName"></param>
    public static async void LoadScene(Scene targetScene)
    {
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        await Task.Delay(100);
        var slider = FindObjectOfType<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider not found in the loading scene.");
            return;
        }
        var asyncLoad = SceneManager.LoadSceneAsync(targetScene.ToString());
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            slider.value = asyncLoad.progress;
            await Task.Delay(100); 
        }
        slider.value = 1.0f;
        await Task.Delay(500);
        asyncLoad.allowSceneActivation = true;
    }
    
    public static async void LoadSceneNetwork(Scene targetScene)
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
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);    
        }
    }
    
    
    public static void LoadNetwork(Scene targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
}
