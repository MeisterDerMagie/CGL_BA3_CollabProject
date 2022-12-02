//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

//attempt to reconnect to the server provider if the previous connection attemt wasn't successfull
public class AttemptServerProviderReconnect : MonoBehaviour
{
    [SerializeField] private float attemptInterval = 3f;
    private Thread _connectionThread;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        ServerProviderClient.OnCouldNotConnectToServerProvider += OnConnectionFailed;
    }

    private void OnConnectionFailed()
    {
        Timing.RunCoroutine(_WaitThenStartNewAttempt());
    }

    [Button]
    private void StartConnectionAttempt()
    {
        Debug.Log("<color=#25aaef>Attempt reconnect to server provider.</color>");
        
        //do nothing if the previous attempt is still waiting for a server response
        if (_connectionThread != null && _connectionThread.IsAlive)
            return;

        //start connection attempt in new thread
        var threadStart = new ThreadStart(ServerProviderClient.ConnectClient);
        _connectionThread = new Thread(threadStart);
        _connectionThread.Start();
    }

    private IEnumerator<float> _WaitThenStartNewAttempt()
    {
        yield return Timing.WaitForSeconds(attemptInterval);
        
        StartConnectionAttempt();
    }

    private void OnDestroy()
    {
        _connectionThread?.Abort();
        ServerProviderClient.OnCouldNotConnectToServerProvider -= OnConnectionFailed;

    }
}