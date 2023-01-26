using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Podium_PlayComposition : NetworkBehaviour
{
    [SerializeField] private PlaybackVoting playbackVoting;
    [SerializeField] private Podium_PromptDisplay promptDisplay;
    [SerializeField] private Transform playButton, stopButton;
    
    private NetworkList<Eighth> _composition;
    private List<Podium_PlayComposition> _allPodiumPlayCompositions = new();

    private void Awake()
    {
        _composition = new NetworkList<Eighth>();

        _allPodiumPlayCompositions = FindObjectsOfType<Podium_PlayComposition>().ToList();
        
        //stop when playback ended
        playbackVoting.onPlaybackEnded += Stop;
    }

    public override void OnDestroy()
    {
        playbackVoting.onPlaybackEnded -= Stop;
    }

    public void SetComposition(List<Eighth> composition)
    {
        if (!NetworkManager.IsServer)
        {
            Debug.LogError("This should only be called on the server.", this);
        }
        
        _composition.Clear();
        foreach (Eighth eighth in composition)
        {
            _composition.Add(eighth);
        }
    }
    
    public void Play()
    {
        if (_composition.Count == 0) return;
    
        //show prompt
        promptDisplay.Show();

        //switch button from play to stop
        playButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);

        //convert NetworkList to list
        var composition = new List<Eighth>();
        foreach (Eighth eighth in _composition)
        {
            composition.Add(eighth);
        }

        //stop playback of all other podiums
        StopPlaybackOfAllOtherPodiums();
        
        //start playback
        playbackVoting.StartPlayback(composition);
    }

    public void Stop()
    {
        //hide prompt
        promptDisplay.Hide();
        
        //switch button from stop to play
        playButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        
        //stop playback
        playbackVoting.StopPlayback();
    }

    private void StopPlaybackOfAllOtherPodiums()
    {
        foreach (Podium_PlayComposition podium in _allPodiumPlayCompositions)
        {
            if(podium != this) podium.Stop();
        }
    }
}