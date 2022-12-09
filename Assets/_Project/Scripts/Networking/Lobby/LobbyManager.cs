//(c) copyright by Martin M. Klöckener
using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using Wichtel;

public class LobbyManager : MonoBehaviour
{
    [DisplayAsString] public string lobbyCode;
    
    #region Singleton
    private static LobbyManager instance;
    public static LobbyManager Instance => instance;
    
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
    private void ResetLobbyCode() => lobbyCode = string.Empty;
}
