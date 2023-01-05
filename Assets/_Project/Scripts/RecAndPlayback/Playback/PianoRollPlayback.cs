using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

/// <summary>
/// This is the Piano Roll script for the playback stage.
/// keeps track separately of which stage the preview is in and which state the timeline is in (where we actually are in sync with the music).
/// at the start --> finds all players in the scene and then after a few seconds starts playing back everyone's recordings, leaving barsBetween amount of bars between the playbacks
/// tells Display script when to update to the next player's data (prompt, name, image)
/// after playing back the last player's recording --> set to ready and move on
/// </summary>

public class PianoRollPlayback : NetworkBehaviour
{
    private enum PlaybackStage
    {
        IDLE,
        COUNTIN,
        PLAYING
    }

    [SerializeField] private CharDisplayPB _display;
    private PianoRollTimer _timer;
    private AudioRoll _audioRoll;
    private NoteSpawner _spawner;
    [SerializeField] private Light _light;
    [SerializeField] private LoadNextSceneWhenAllClientsAreDone _loadNext;

    [Space]
    public float bpm = 110f;
    [Tooltip("amount of bars before first bar")]
    [SerializeField] int countIn = 2;
    [Tooltip("amount of bars between bars")]
    [SerializeField] int barsBetween = 3;
    [Tooltip("seconds before playing back at start of scene")]
    [SerializeField] float time = 2.5f;

    public List<PlayerData> playerDatas;
    int timelinePlayer;
    int previewPlayer;

    int timelineBar;
    int previewBar;

    private PlaybackStage timelineStage;
    private PlaybackStage previewStage;
    private bool playingBack;

    public override void OnNetworkSpawn()
    {
        // find all players and their data
        playerDatas = FindObjectsOfType<PlayerData>().ToList();
        timelinePlayer = 0;
        previewPlayer = 0;

        // set scripts
        _timer = GetComponent<PianoRollTimer>();
        _audioRoll = GetComponentInChildren<AudioRoll>();
        _spawner = GetComponent<NoteSpawner>();

        // set up playing back to false --> don't start too soon
        playingBack = false;
        timelineStage = PlaybackStage.IDLE;
        previewStage = PlaybackStage.IDLE;

        //
        _light.TurnOff();
        _display.TurnOffCharacter();

        // wait short time for playback to start
        StartCoroutine(WaitToStart());
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(time);
        StartPlayback();
    }

    public void StartPlayback()
    {
        // start backing track
        GetComponentInChildren<BackingTrack>().StartMusic();
        _spawner.ActivateIdleLines(false);
        _spawner.SpawnLinesOnRoll(bpm);

        // set playing Back to true + only spawn active lines
        playingBack = true;
        _spawner.spawnActive = true;

        timelineBar = _timer.timelineBar;
        previewBar = _timer.previewBar;

        // set currentPlayer to zero
        timelinePlayer = 0;
        previewPlayer = 0;

        // set current stages
        timelineStage = PlaybackStage.COUNTIN;
        previewStage = PlaybackStage.COUNTIN;

        // update display and switch off light
        _light.TurnOff();
    }

    // only needed for testing
    public void StopPlayback()
    {
        playingBack = false;
        GetComponentInChildren<BackingTrack>().StopMusic();
        timelineStage = PlaybackStage.IDLE;
        previewStage = PlaybackStage.IDLE;

        _spawner.ActivateIdleLines(true);
        _spawner.ActivateLines(false);
        _spawner.DeleteActiveLines();
        _spawner.DeleteActiveNotes();

        _light.TurnOff();
        _display.TurnOffCharacter();
    }

    public void NextBeat()
    {
        if (!playingBack) return;
        if (_timer.timelineBeat == 0) return;
        
        PlayQuarterNote();

        // if timeline beat is 1 --> see for next stage
        if (_timer.timelineBeat == 1)
        {
            UpdateTimelineStage();
        }
        // if preview beat is 1 --> see for next stage
        else if (_timer.previewBeat == 1)
        {
            UpdatePreviewStage();
        }

        // update display
        if (previewStage == PlaybackStage.PLAYING)
            if (playerDatas[previewPlayer].Recording[_timer.previewBeat - 1].contains)
                _spawner.SpawnNote(playerDatas[previewPlayer].Recording[_timer.previewBeat - 1].instrumentID, bpm);

        // play audio
        if (timelineStage == PlaybackStage.PLAYING)
            if (playerDatas[timelinePlayer].Recording[_timer.timelineBeat - 1].contains)
                _audioRoll.PlaySound(playerDatas[timelinePlayer].Recording[_timer.timelineBeat - 1].instrumentID);

    }

    void UpdateTimelineStage()
    {
        switch (timelineStage)
        {
            case PlaybackStage.IDLE:
                // do nothing, shouldn't even be here
                break;
            case PlaybackStage.COUNTIN:
                
                if (timelinePlayer == 0)
                {
                    if (_timer.timelineBar - timelineBar == countIn)
                    {
                        UpdatePlayer();
                    }
                    if (_timer.timelineBar - timelineBar == countIn + 1)
                    {
                        // turn on light and change state to playing
                        _light.TurnOn();
                        timelineStage = PlaybackStage.PLAYING;
                        timelineBar = _timer.timelineBar;
                    }
                }
                else
                {
                    if (_timer.timelineBar - timelineBar == 1)
                    {
                        // Update visuals to new player
                        UpdatePlayer();
                    }

                    if (_timer.timelineBar - timelineBar == barsBetween)
                    {
                        // turn on light and change state to playing
                        _light.TurnOn();
                        timelineStage = PlaybackStage.PLAYING;
                        timelineBar = _timer.timelineBar;
                    }
                }
                break;
            case PlaybackStage.PLAYING:
                timelinePlayer++;
                // check if this was last player
                if (timelinePlayer > playerDatas.Count - 1)
                {
                    LastPlayer();
                }
                else
                {
                    timelineStage = PlaybackStage.COUNTIN;
                    timelineBar = _timer.timelineBar;
                }
                _light.TurnOff();
                //_display.TurnOffCharacter();
                break;
            default:
                break;
        }
    }

    void UpdatePreviewStage()
    {
        switch (previewStage)
        {
            case PlaybackStage.IDLE:
                // do nothing
                break;
            case PlaybackStage.COUNTIN:
                if (previewPlayer == 0)
                {
                    if (_timer.previewBar - previewBar == countIn - 1)
                    {
                        previewStage = PlaybackStage.PLAYING;
                        previewBar = _timer.previewBar;
                    }
                }
                else
                {
                    if (_timer.previewBar - previewBar == barsBetween)
                    {
                        previewStage = PlaybackStage.PLAYING;
                        previewBar = _timer.previewBar;
                    }
                }
                break;
            case PlaybackStage.PLAYING:
                previewPlayer++;
                // check if last player
                if (previewPlayer == playerDatas.Count - 1)
                {
                    previewStage = PlaybackStage.IDLE;
                }
                else
                {
                    previewStage = PlaybackStage.COUNTIN;
                    previewBar = _timer.previewBar;
                }
                break;
            default:
                break;
        }
    }

    void UpdatePlayer()
    {
        // tell display to update character prompt, name and image:
        _display.SetCharacterDisplay(playerDatas[timelinePlayer].AssignedPrompt, 
            playerDatas[timelinePlayer].PlayerName, 
            CharacterManager.Instance.GetCharacter(playerDatas[timelinePlayer].CharacterId).characterImage);
    }

    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (_timer.previewBeat == 1) _spawner.SpawnLine(bpm, 1);
        if (_timer.previewBeat == 3) _spawner.SpawnLine(bpm, 2);
        if (_timer.previewBeat == 5) _spawner.SpawnLine(bpm, 3);
        if (_timer.previewBeat == 7) _spawner.SpawnLine(bpm, 4);
    }

        void LastPlayer()
    {
        // if it was last player
        timelineStage = PlaybackStage.IDLE;
        playingBack = false;

        _light.TurnOff();
        _display.TurnOffCharacter();

        if (!NetworkManager.IsServer)
            _loadNext.Done();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        //GUILayout.Box($"Current timeline stage: {timelineStage} + current preview stage {previewStage}, current player: {timelinePlayer}");
    }
#endif
}

