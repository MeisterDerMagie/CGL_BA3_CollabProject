//(c) copyright by Martin M. Klöckener
using System;
using Unity.Netcode;
using UnityEngine;

public class ConnectionApproval : MonoBehaviour
{
    [SerializeField] private uint maxPlayers = 8;
    [HideInInspector] public bool gameIsInLobby = true;
    
    public bool ServerIsFull => NetworkManager.Singleton.ConnectedClientsIds.Count >= maxPlayers;
    
    #region Singleton
    private static ConnectionApproval instance;
    public static ConnectionApproval Instance => instance;
    
    private void Awake()
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
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Approval check!");
        
        //don't allow more than max players
        if (ServerIsFull)
        {
            response.Approved = false;
            Debug.Log("More than 8 players tried to join. Don't approve connection.");
        }
        
        //only allow joining during the lobby scene
        else if (!gameIsInLobby)
        {
            response.Approved = false;
            Debug.Log("Player tried to connect while the game is already running. Don't approve connection.");
        }
        
        else
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Pending = false;
        }
    }

}