using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipSceneButton : MonoBehaviour
{
    [SerializeField] private NetworkSceneLoader nextScene;
    
    private void OnGUI()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        
        //show buttons
        if(GUILayout.Button("Skip Scene"))
            NetworkSceneLoading.LoadNetworkScene(nextScene, LoadSceneMode.Single);

        GUILayout.EndArea();
    }
}
