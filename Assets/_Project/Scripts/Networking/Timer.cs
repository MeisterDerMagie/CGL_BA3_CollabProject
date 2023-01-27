//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using MEC;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Timer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI timerView;
    [SerializeField] private float duration;
    [SerializeField] private float syncInterval = 3f;
    [SerializeField] private bool startOnNetworkSpawn = true;
    [SerializeField] private UnityEvent onTimerEndedServer, onTimerEndedClient;

    [SerializeField] private EventReference tenSeconds, threeSeconds, timeUp;
    
    public event Action OnTimerEndedServer = delegate {  };
    public event Action OnTimerEndedClient = delegate {  };
    
    private NetworkVariable<float> _timer = new NetworkVariable<float>();

    private float _timerLocal;
    private CoroutineHandle _timerCoroutine;

    private bool _playedTenSecondsSound, _playedThreeSecondsSound, _playedTwoSecondsSound, _playedOneSecondSound;

    public override void OnNetworkSpawn()
    {
        ResetTimer();
        
        //start the syncing
        if (IsServer)
        {
            Timing.RunCoroutine(_SyncTimer().CancelWith(gameObject));

            if (startOnNetworkSpawn)
            {
                StartTimer();
            }
        }
        
        if(!IsServer) _timer.OnValueChanged += OnValueChanged;
    }
    
    //starts or resumes timer
    public void StartTimer()
    {
        //if the timer has already ended, we can't start it
        if(_timerLocal == 0f)
        {
            Debug.LogWarning("You're trying to start a timer that has already ended. If you intended to restart it, call ResetTimer() before.", this);
            return;
        }
        else
        {
            _timerCoroutine = Timing.RunCoroutine(_RunTimer().CancelWith(gameObject));
        }
    }
    //doesn't pause or unpause
    public void ResetTimer()
    {
        _timerLocal = duration;
        
        if (IsServer)
        {
            //sync the timer
            SyncTimer();
        }

        UpdateView();
    }

    private void OnValueChanged(float previousvalue, float newvalue)
    {
        _timerLocal = newvalue;
        UpdateView();
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer) return;
        
        //count down the timer on the client
        _timerLocal = Mathf.Max(_timerLocal - Time.deltaTime, 0f);
        UpdateView();
        
        //play sound when timer reaches 10 seconds
        if (_timerLocal <= 11f && !_playedTenSecondsSound)
        {
            RuntimeManager.PlayOneShot(tenSeconds);
            _playedTenSecondsSound = true;
        }
        //play sound when timer reaches 3 seconds
        else if (_timerLocal <= 4f && !_playedThreeSecondsSound)
        {
            RuntimeManager.PlayOneShot(threeSeconds);
            _playedThreeSecondsSound = true;
        }
        else if (_timerLocal <= 3f && !_playedTwoSecondsSound)
        {
            RuntimeManager.PlayOneShot(threeSeconds);
            _playedTwoSecondsSound = true;
        }
        else if (_timerLocal <= 2f && !_playedOneSecondSound)
        {
            RuntimeManager.PlayOneShot(threeSeconds);
            _playedOneSecondSound = true;
        }
    }

    private IEnumerator<float> _RunTimer()
    {
        while (true)
        {
            //if it's not paused, decrease timer and ensure that it's never smaller than 0
            _timerLocal = Mathf.Max(_timerLocal - Time.deltaTime, 0f);
            
            //if the timer reached 0, call the events
            if (_timerLocal == 0f)
            {
                //sync the timer
                SyncTimer();
                
                //timer ended: call events on server
                OnTimerEndedServer?.Invoke();
                onTimerEndedServer.Invoke();
                
                //then call events on client
                OnTimerEndedClientRpc();

                //stop the timer
                yield break;
            }
            
            //update view
            UpdateView();
            

            //wait one frame
            yield return Timing.WaitForOneFrame;
        }
    }

    private IEnumerator<float> _SyncTimer()
    {
        while (true)
        {
            //sync the time all x seconds
            SyncTimer();
            yield return Timing.WaitForSeconds(syncInterval);
        }
    }

    private void SyncTimer()
    {
        _timer.Value = _timerLocal;
    }

    private void UpdateView()
    {
        if (timerView == null) return;
        
        timerView.SetText(FormatTimer(_timerLocal));
    }

    private string FormatTimer(float seconds)
    {
        int secondsInt = Mathf.FloorToInt(_timerLocal);
        var timespan = new TimeSpan(0, 0, 0, secondsInt);

        return timespan.ToString(@"mm\:ss");
    }

    [ClientRpc]
    private void OnTimerEndedClientRpc()
    {
        //play sound
        RuntimeManager.PlayOneShot(timeUp);
        
        //timer ended: call events on client
        OnTimerEndedClient?.Invoke();
        onTimerEndedClient.Invoke();
    }
}