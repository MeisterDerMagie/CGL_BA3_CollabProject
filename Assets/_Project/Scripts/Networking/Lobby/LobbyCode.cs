//(c) copyright by Martin M. Klöckener
using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using Wichtel;

public class LobbyCode : MonoBehaviour
{
    [DisplayAsString] public string code;
    
    #region Singleton
    private static LobbyCode instance;
    public static LobbyCode Instance => instance;
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Debug.LogWarning("Singleton instance already exists. You should never initialize a second one.", this);
    }
    #endregion

    private void Start()
    {
        #if !UNITY_SERVER
        NetworkEvents.OnClientShutdown += ResetLobbyCode;
        #endif
    }

    //Reset lobby code
    private void ResetLobbyCode() => code = string.Empty;
}
