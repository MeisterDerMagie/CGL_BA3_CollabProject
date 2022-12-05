//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLobbySceneAfterServerStarted : NetworkBehaviour
{
    [SerializeField] private SceneReference lobbyScene;
    
    #if UNITY_SERVER
    public override void OnNetworkSpawn()
    {
        //load lobby scene
        NetworkManager.SceneManager.LoadScene(lobbyScene, LoadSceneMode.Single);
    }
    #endif
}
