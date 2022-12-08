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
    
    #if !UNITY_SERVER
    public override void OnDestroy()
    {
        if(IsLocalPlayer) mainMenuLoader.Load();
    }
    #endif
}
