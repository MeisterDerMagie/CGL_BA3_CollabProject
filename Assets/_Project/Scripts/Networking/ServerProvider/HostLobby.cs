using System;
using System.Collections;
using System.Collections.Generic;
using Doodlenite;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wichtel.SceneManagement;

namespace Doodlenite {
public class HostLobby : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorMessage;
    
    public void Host() => ServerProviderCommunication.Instance.HostRequest();

    public void CancelHostRequest() => ServerProviderCommunication.Instance.CancelHostRequest();

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnConnectionAttemptFailed;
        ServerProviderClient.OnHostGameFailed += OnHostGameFailed;
    }

    private void OnEnable()
    {
        //hide error message
        errorMessage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnConnectionAttemptFailed;
        ServerProviderClient.OnHostGameFailed -= OnHostGameFailed;

    }

    private void OnHostGameFailed(string reason)
    {
        ShowErrorMessage(reason);
    }

    private void OnConnectionAttemptFailed(ulong clientId)
    {
        ShowErrorMessage("Could not connect to the game server. Please try again.");
    }

    private void ShowErrorMessage(string message)
    {
        errorMessage.SetText(message);
        errorMessage.gameObject.SetActive(true);
    }
}
}