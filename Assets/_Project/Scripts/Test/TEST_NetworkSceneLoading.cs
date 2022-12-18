//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TEST_NetworkSceneLoading : MonoBehaviour
{
    private bool initialized = false;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (initialized) return;

        if (NetworkManager.Singleton == null)
        {
            initialized = false;
            return;
        }

        if (NetworkManager.Singleton.SceneManager == null)
        {
            initialized = false;
            return;
        }
        
        NetworkManager.Singleton.SceneManager.OnLoad += OnLoad;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventComplete;
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
        NetworkManager.Singleton.SceneManager.OnSynchronize += OnSynchronize;
        NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;
        NetworkManager.Singleton.SceneManager.OnUnload += OnUnload;
        NetworkManager.Singleton.SceneManager.OnUnloadComplete += OnUnloadComplete;
        NetworkManager.Singleton.SceneManager.OnUnloadEventCompleted += OnUnloadEventComplete;

        initialized = true;
    }

    private void OnUnloadEventComplete(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnUnloadEventComplete");
    }

    private void OnUnloadComplete(ulong clientid, string scenename)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnUnloadComplete");
    }

    private void OnUnload(ulong clientid, string scenename, AsyncOperation asyncoperation)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnUnload");
    }

    private void OnSynchronizeComplete(ulong clientid)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnSynchronizeComplete");
    }

    private void OnSynchronize(ulong clientid)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnSynchronize");
    }

    private void OnSceneEvent(SceneEvent sceneevent)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnSceneEvent");
    }

    private void OnLoadEventComplete(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnLoadEventComplete");
    }

    private void OnLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnLoadComplete");
    }

    private void OnLoad(ulong clientid, string scenename, LoadSceneMode loadscenemode, AsyncOperation asyncoperation)
    {
        string serverOrClient = NetworkManager.Singleton.IsServer ? "Server:" : "Client:";
        Debug.Log($"{serverOrClient} OnLoad");
    }
}