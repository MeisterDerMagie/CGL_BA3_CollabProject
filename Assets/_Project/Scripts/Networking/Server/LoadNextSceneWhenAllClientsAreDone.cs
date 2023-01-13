//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextSceneWhenAllClientsAreDone : NetworkBehaviour
{
    [SerializeField] private NetworkSceneLoader nextScene;
    
    private Dictionary<ulong, bool> _clientStates = new Dictionary<ulong, bool>();
    private bool _sceneIsLoading;

    public override void OnNetworkSpawn()
    {
        //only run on server
        if (!NetworkManager.IsServer) return;
        
        //add all clients to the dictionary
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            _clientStates.Add(clientId, false);
        }
        
        //subscribe to client disconnect event
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    //call this on the client when they are done with the current scene
    public void Done()
    {
        if (NetworkManager == null) return;
        if (NetworkManager.IsServer)
        {
            Debug.LogWarning("Don't call this method on the server.", this);
            return;
        }
        
        DoneServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DoneServerRpc(ulong clientId)
    {
        SetClientDone(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        //treat disconnected clients as done
        SetClientDone(clientId);
    }

    private void SetClientDone(ulong clientId)
    {
        //set client state
        if (_clientStates.ContainsKey(clientId))
            _clientStates[clientId] = true;
        
        //if all clients are done, load next scene
        bool allClientsDone = true;

        foreach (KeyValuePair<ulong, bool> clientState in _clientStates)
        {
            if (clientState.Value == false)
            {
                allClientsDone = false;
                break;
            }
        }

        if (allClientsDone && !_sceneIsLoading)
        {
            _sceneIsLoading = true;
            LoadNextScene();
        }

        Debug.LogWarning("WATCH OUT! HERE COULD BE A PROBLEM WITH ONCLIENTDISCONNECT. DOES IT GET CALLED EVEN AFTER THE NEW SCENE WAS LOADED?");
    }

    private void LoadNextScene()
    {
        NetworkSceneLoading.LoadNetworkScene(nextScene, LoadSceneMode.Single);
    }
}