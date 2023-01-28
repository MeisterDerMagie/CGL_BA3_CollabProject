using System;
using System.Collections;
using System.Collections.Generic;
using Doodlenite;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wichtel.SceneManagement;

namespace Doodlenite {
public class HostLobby : MonoBehaviour
{
    [SerializeField] private Transform errorMessage;
    
    public void Host() => ServerProviderCommunication.Instance.HostRequest();

    public void CancelHostRequest() => ServerProviderCommunication.Instance.CancelHostRequest();

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += ShowErrorMessage;
    }

    private void OnEnable()
    {
        //hide error message
        errorMessage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= ShowErrorMessage;
    }

    private void ShowErrorMessage(ulong clientId)
    {
        errorMessage.gameObject.SetActive(true);
    }
}
}