using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This Class can be used as a singleton and has the control info and functionalities that will be needed in
/// the Campus scene 
/// </summary>
public class CampusController : MonoBehaviour
{
    public Camera mainCamera;
    public Camera avatarCustomizationCamera;
    public Canvas mainCanvas;
    public Canvas avatarCustomizationCanvas;
    private GameManager _gameManager;
    
    private static CampusController _instance;
    public static CampusController Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        Time.timeScale = 1;
        _gameManager = GameManager.Instance;
    }

    private async void Start()
    {
        GameManager.Instance.AudioManager.Play(SoundName.CampusTheme);
        bool isGameStarted = PlayerPrefsManager.GetBool(PlayerPrefsKeys.GameStarted, false);
        if (!isGameStarted)
        {
            ChangeMode(toAvatarCustomizationMode: true);
        }
        await InitializeUnityServices();
        await SignInAnonymously();
        // _gameManager.AudioManager.play(SoundName.CampusArea);
        // DialogueController.Show(1);
    }
    
    
    private async Task InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("Unity Services Initialized");
    }

    private async Task SignInAnonymously()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
        }
    }

    public void ChangeMode(bool toAvatarCustomizationMode)
    {
        if (toAvatarCustomizationMode)
        {
            mainCamera.gameObject.SetActive(false);
            avatarCustomizationCamera.gameObject.SetActive(true);
            mainCanvas.gameObject.SetActive(false);
            avatarCustomizationCanvas.gameObject.SetActive(true);
        }
        else
        {
            mainCamera.gameObject.SetActive(true);
            avatarCustomizationCamera.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(true);
            avatarCustomizationCanvas.gameObject.SetActive(false);
        }
    }

}
