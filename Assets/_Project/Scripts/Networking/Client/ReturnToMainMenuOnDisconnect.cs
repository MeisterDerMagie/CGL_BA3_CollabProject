using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Wichtel.SceneManagement;

//return to the main menu in case we disconnect
public class ReturnToMainMenuOnDisconnect : NetworkBehaviour
{
    [SerializeField] private SceneLoader mainMenuLoader;
    private bool _wasClientBefore;
    
    #if !UNITY_SERVER
    private void Start() => DontDestroyOnLoad(gameObject);

    private void Update()
    {
        Debug.Log($"IsClient: {IsClient}");
        
        if(!IsClient && _wasClientBefore) mainMenuLoader.Load();
    }
    #endif
}
