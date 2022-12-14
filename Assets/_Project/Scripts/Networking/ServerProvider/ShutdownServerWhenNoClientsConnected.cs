using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class ShutdownServerWhenNoClientsConnected : NetworkBehaviour
{
    [SerializeField, SuffixLabel("seconds")] private float shutdownAfter = 10f;
    [SerializeField] private bool isEnabled = true;
    private CoroutineHandle _coroutine;

    #if UNITY_SERVER
    public override void OnNetworkSpawn()
    {
        if (!isEnabled) return;
        _coroutine = Timing.RunCoroutine(_WaitThenShutdown());
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!isEnabled) return;
        
        if (NetworkManager.ConnectedClients.Count > 0 && _coroutine.IsRunning)
            Timing.KillCoroutines(_coroutine);

        else if (NetworkManager.ConnectedClients.Count == 0 && !_coroutine.IsRunning)
            _coroutine = Timing.RunCoroutine(_WaitThenShutdown());
    }

    private IEnumerator<float> _WaitThenShutdown()
    {
        yield return Timing.WaitForSeconds(shutdownAfter);

        Debug.Log("Shutting down server due to inactivity.");
        Shutdown();
    }

    private void Shutdown()
    {
        ServerProviderCommunication.Instance.ServerStopped();
        ServerProviderClient.DisconnectClient();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    #endif
}
