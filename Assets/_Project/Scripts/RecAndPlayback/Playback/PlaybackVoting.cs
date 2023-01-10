using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

/// <summary>
/// controls the audio playback during the voting stage (when players vote for other's creativity)
/// called via button --> set waitToStart to true and set playerID
/// on the next 1 (via timer script) set playback to true and then play on every beat that the player's bar has a note
/// </summary>

public class PlaybackVoting : MonoBehaviour
{
    private BackingTrack _backingTrack;
    private PianoRollTimer _timer;
    private AudioRoll _audioRoll;

    private bool playback;
    private bool waitToStart;

    private List<List<Eighth>> recording;
    private int timer;

    void Start()
    {
        _backingTrack = GetComponent<BackingTrack>();
        _timer = GetComponent<PianoRollTimer>();
        _audioRoll = GetComponent<AudioRoll>();

        recording = new List<List<Eighth>>();

        waitToStart = false;
        playback = false;

        StartCoroutine(WaitToStart());
        //_audioRoll.TestSetup();
        _audioRoll.SetUpAllInstances();
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(1f);

        _backingTrack.StartMusic();
    }

    public void NextBeat()
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        if (_timer.timelineBeat == 1)
        {
            if (waitToStart)
            {
                waitToStart = false;
                playback = true;
            }
            else
            {
                timer++;
                if (timer > Constants.RECORDING_LENGTH - 1) timer = 0;
            }
        }

        if (playback)
            if (recording[timer][_timer.timelineBeat - 1].contains)
                _audioRoll.PlayerInputSound(PlayerData.LocalPlayerData.InstrumentIds.IndexOf(recording[timer][_timer.timelineBeat - 1].instrumentID));
    }

    public void StartPlayback(List<Eighth> _recording)
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        waitToStart = true;
        timer = 0;

        // transform list of eighths into different bars
        recording.Clear();

        // for as many times as the amount of bars we recorded
        for (int b = 0; b < Constants.RECORDING_LENGTH; b++)
        {
            List<Eighth> bar = new List<Eighth>();

            // go through every eighth
            for (int i = 0; i < 8; i++)
            {
                // and add the eighth at eight + which bar we're in to the List of Eights
                bar.Add(_recording[i + b * 8]);
            }

            recording.Add(bar);
        }
    }

    public void StopPlayback()
    {
        waitToStart = false;
        playback = false;
    }
}
