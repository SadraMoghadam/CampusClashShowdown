using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CampusSceneNetworkCleanUp : MonoBehaviour
{
    private void Awake() {
        if (NetworkManager.Singleton != null) 
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (MultiplayerController.Instance != null) 
        {
            Destroy(MultiplayerController.Instance.gameObject);
        }

        if (NetworkLobby.Instance != null)
        {
            Destroy(NetworkLobby.Instance.gameObject);    
        }
    }
}
