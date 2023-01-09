using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public List<PlayerData> playerDatas;
    //public Bar[] bar;

    private bool playback;
    private bool waitToStart;

    int playerID;

    void Start()
    {
        _backingTrack = GetComponent<BackingTrack>();
        _timer = GetComponent<PianoRollTimer>();
        _audioRoll = GetComponent<AudioRoll>();

        // find all players and their data
        playerDatas = FindObjectsOfType<PlayerData>().ToList();

        waitToStart = false;
        playback = false;

        playerID = -1;

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

        if (_timer.timelineBeat == 1 && waitToStart)
        {
            waitToStart = false;
            playback = true;
        }

        if (playback)
        {
            /* -- for testing
            if (bar[playerID].eighth[_timer.timelineBeat - 1].contains)
                _audioRoll.TestSound(bar[playerID].eighth[_timer.timelineBeat - 1].instrumentID);
            */
            if (playerDatas[playerID].Recording[_timer.timelineBeat - 1].contains)
                _audioRoll.PlayerInputSound(playerDatas[playerID].Recording[_timer.timelineBeat - 1].instrumentID);
        }
    }

    // umarbeiten auf--> List<Eighth> recording wird geschickt
    public void StartPlayback(int _playerID)
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        // if the same button is pressed and we're playing back --> stop
        if (_playerID == playerID && (playback || waitToStart))
        {
            playerID = _playerID;
            playback = false;
            waitToStart = false;
        }
        // otherwise stop and start with different player
        else
        {
            playerID = _playerID;
            waitToStart = true;
            playback = false;
        }
    }

    public void StopPlayback()
    {
        waitToStart = false;
        playback = false;
    }
}
