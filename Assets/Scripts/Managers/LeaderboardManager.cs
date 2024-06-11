using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    private int _leaderboardMaxEntryCount = 50;

    private const string LeaderboardId = "leaderboard";


    private async void Awake()
    {
        // await InitializeUnityServices();
        // await SignInAnonymously();
    }

    private void Start()
    {
        
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
            AuthenticationService.Instance.UpdatePlayerNameAsync("Amir");
            AddOrUpdateScore(252);
        }
    }

    public async Task FetchAndDisplayLeaderboard(LeaderboardUI leaderboardUI)
    {
        try
        {
            var results = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId,
                new GetScoresOptions { Limit = _leaderboardMaxEntryCount });
            PopulateLeaderboard(results, leaderboardUI);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching leaderboard: {e.Message}");
        }
    }

    private void PopulateLeaderboard(LeaderboardScoresPage results, LeaderboardUI leaderboardUI)
    {
        foreach (Transform child in leaderboardUI.contentPanel)
        {
            Destroy(child.gameObject); // Clear existing entries
        }

        foreach (var entry in results.Results)
        {
            GameObject newEntry = Instantiate(leaderboardUI.entryPrefab, leaderboardUI.contentPanel);
            LeaderboardEntryUI newLeaderboardEntry = newEntry.GetComponent<LeaderboardEntryUI>();
            newLeaderboardEntry.rank.text = (entry.Rank + 1).ToString();
            newLeaderboardEntry.username.text = entry.PlayerName;
            newLeaderboardEntry.resourceCount.text = entry.Score.ToString();
        }
    }

    public async void AddOrUpdateScore(int resourceCount)
    {
        try
        {
            AddPlayerScoreOptions options = new AddPlayerScoreOptions() {};
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, resourceCount);
            
            Debug.Log(scoreResponse);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error submitting score: {e.Message}");
        }
    }
    
    public async Task<double> GetPlayerScore()
    {
        try
        {
            Debug.Log("Attempting to fetch player score...");
            var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);

            if (scoreResponse != null)
            {
                Debug.Log($"Player score: {scoreResponse.Score}");
                return scoreResponse.Score;
            }
            else
            {
                Debug.Log("No score found for the player on the leaderboard.");
                return 0f;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error fetching score: {e.Message}\n{e.StackTrace}");
            if (e.Message.Contains("Leaderboard entry could not be found"))
            {
                return 0f;
            }
            else
            {
                return -1f;
            }
        }
    }

//     public async Task FetchAndDisplayLeaderboard(LeaderboardUI leaderboardUI)
//     {
//         try
//         {
//             var response = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { LeaderboardKey });
//             if (response.ContainsKey(LeaderboardKey))
//             {
//                 string json = response[LeaderboardKey];
//                 List<LeaderboardEntry> leaderboardEntries = JsonUtility.FromJson<LeaderboardEntryList>($"{json}").entries;
//                 PopulateLeaderboard(leaderboardUI, leaderboardEntries);
//             }
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"Error fetching leaderboard: {e.Message}");
//         }
//     }
//
//     private void PopulateLeaderboard(LeaderboardUI leaderboardUI, List<LeaderboardEntry> entries)
//     {
//         foreach (var entry in entries)
//         {
//             GameObject newEntry = Instantiate(leaderboardUI.entryPrefab, leaderboardUI.contentPanel);
//             LeaderboardEntryUI newLeaderboardEntry = newEntry.GetComponent<LeaderboardEntryUI>();
//             newLeaderboardEntry.username.text = entry.username;
//             newLeaderboardEntry.resourceCount.text = entry.resourceCount.ToString();
//         }
//     }
//
//     public async void AddNewEntry(string username, int resourceCount, string avatarImage = "")
//     {
//         try
//         {
//             var response = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { LeaderboardKey });
//             List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();
//
//             if (response.ContainsKey(LeaderboardKey))
//             {
//                 string json = response[LeaderboardKey];
//                 leaderboardEntries = JsonUtility.FromJson<LeaderboardEntryList>($"{{\"entries\":{json}}}").entries;
//             }
//
//             bool playerExists = false;
//             foreach (var entry in leaderboardEntries)
//             {
//                 if (entry.username == username)
//                 {
//                     entry.resourceCount = resourceCount;
//                     playerExists = true;
//                     break;
//                 }
//             }
//             
//             if (!playerExists)
//             {
//                 leaderboardEntries.Add(new LeaderboardEntry { avatarImage = avatarImage, username = username, resourceCount = resourceCount });
//             }
//             leaderboardEntries.Sort((x, y) => y.resourceCount.CompareTo(x.resourceCount));
//
//             
//             if (leaderboardEntries.Count > _leaderboardMaxEntryCount)
//             {
//                 leaderboardEntries = leaderboardEntries.GetRange(0, _leaderboardMaxEntryCount);
//             }
//             
//             string newJson = JsonUtility.ToJson(new LeaderboardEntryList { entries = leaderboardEntries });
//             await CloudSaveService.Instance.Data.ForceSaveAsync(new Dictionary<string, object> { { LeaderboardKey, newJson } });
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"Error adding new entry: {e.Message}");
//         }
//     }
//     
//     
//     private void LoadAvatarImage(Image imageComponent, string avatarImageUrl)
//     {
//         StartCoroutine(LoadImageCoroutine(imageComponent, avatarImageUrl));
//     }
//
//     private IEnumerator LoadImageCoroutine(Image imageComponent, string url)
//     {
//         UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
//         yield return request.SendWebRequest();
//         if (request.result == UnityWebRequest.Result.Success)
//         {
//             Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
//             imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
//         }
//         else
//         {
//             Debug.LogError($"Error loading image: {request.error}");
//         }
//     }
// }
}

// [Serializable]
// public class LeaderboardEntry
// {
//     public string avatarImage;
//     public string username;
//     public int resourceCount;
// }
//
// [Serializable]
// public class LeaderboardEntryList
// {
//     public List<LeaderboardEntry> entries;
// }
