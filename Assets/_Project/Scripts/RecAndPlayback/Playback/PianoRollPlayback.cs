using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PianoRollPlayback : MonoBehaviour
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

    public float bpm = 110f;
    [Tooltip("amount of bars before first bar")]
    [SerializeField] int countIn = 2;
    [Tooltip("amount of bars between bars")]
    [SerializeField] int barsBetween = 3;

    public List<PlayerData> playerDatas;
    int playerCount;
    int timelinePlayer;
    int previewPlayer;
    int timelineBar;
    int previewBar;

    private PlaybackStage timelineStage;
    private PlaybackStage previewStage;

    private bool playingBack;

    public List<Bar> bars;
    
    private void Start()
    {
        // find all players and their data
        playerDatas = FindObjectsOfType<PlayerData>().ToList();
        //playerCount = playerDatas.Count;
        playerCount = 8;
        timelinePlayer = 0;
        previewPlayer = 0;

        // set scripts
        _timer = GetComponent<PianoRollTimer>();
        _audioRoll = GetComponentInChildren<AudioRoll>();
        _spawner = GetComponent<NoteSpawner>();

        playingBack = false;
        timelineStage = PlaybackStage.IDLE;
        previewStage = PlaybackStage.IDLE;
        _spawner.ActivateIdleLines(true);

        _display.SwitchLight(false);
        _display.TurnOffCharacter();

        //playerDatas[0].Recording

        // MISSING: start playback
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!playingBack) StartPlayback();
            else StopPlayback();
        }
    }

    public void StartPlayback()
    {
        // start backing track
        GetComponentInChildren<BackingTrack>().StartMusic();
        _spawner.ActivateIdleLines(false);
        _spawner.SpawnLinesOverBar(bpm);

        // set playing Back to true + only spawn active lines
        playingBack = true;
        _spawner.spawnActive = true;

        // set bar counters to zero and one // have to start one below because on first beat it's already updated
        timelineBar = -1;
        previewBar = 0; // because preview is one bar and 2 beats ahead of timeline

        // set currentPlayer to zero
        timelinePlayer = 0;
        previewPlayer = 0;

        // set current stages
        timelineStage = PlaybackStage.COUNTIN;
        previewStage = PlaybackStage.COUNTIN;

        // update display and switch off light
        _display.SwitchLight(false);
    }

    public void StopPlayback()
    {
        playingBack = false;
        GetComponentInChildren<BackingTrack>().StopMusic();
        timelineStage = PlaybackStage.IDLE;
        previewStage = PlaybackStage.IDLE;
        _spawner.ActivateIdleLines(true);
        _spawner.ActivateLines(false);

        _display.SwitchLight(false);
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
            timelineBar++;
            UpdateTimelineStage();
        }
        // if preview beat is 1 --> see for next stage
        else if (_timer.previewBeat == 1)
        {
            previewBar++;
            UpdatePreviewStage();
        }

        // update display
        if (previewStage == PlaybackStage.PLAYING)
            PreviewNote();

        // play audio
        if (timelineStage == PlaybackStage.PLAYING)
            if (bars[timelinePlayer].eighth[_timer.timelineBeat - 1].contains)
                _audioRoll.PlaySound(bars[timelinePlayer].eighth[_timer.timelineBeat - 1].instrumentID);

    }

    void UpdateTimelineStage()
    {
        switch (timelineStage)
        {
            case PlaybackStage.IDLE:
                // do nothing, shouldn't even be here
                break;
            case PlaybackStage.COUNTIN:
                if (timelineBar == 1)
                {
                    // Update visuals to new player
                    UpdatePlayer();
                }
                if (timelinePlayer == 0)
                {
                    if (timelineBar == countIn)
                    {
                        // turn on light and change state to playing
                        _display.SwitchLight(true);
                        timelineStage = PlaybackStage.PLAYING;
                        timelineBar = 0;
                    }
                }
                else
                {
                    if (timelineBar == barsBetween)
                    {
                        // turn on light and change state to playing
                        _display.SwitchLight(true);
                        timelineStage = PlaybackStage.PLAYING;
                        timelineBar = 0;
                    }
                }
                break;
            case PlaybackStage.PLAYING:
                timelinePlayer++;
                // check if this was last player
                if (timelinePlayer > playerCount - 1)
                {
                    LastPlayer();
                }
                else
                {
                    timelineStage = PlaybackStage.COUNTIN;
                    timelineBar = 0;
                }
                _display.SwitchLight(false);
                _display.TurnOffCharacter();
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
                    if (previewBar == countIn - 1)
                    {
                        previewStage = PlaybackStage.PLAYING;
                        previewBar = 0;
                    }
                }
                else
                {
                    if (previewBar == barsBetween)
                    {
                        previewStage = PlaybackStage.PLAYING;
                        previewBar = 0;
                    }
                }
                break;
            case PlaybackStage.PLAYING:
                previewPlayer++;
                // check if last player
                if (previewPlayer == playerCount - 1)
                {
                    previewStage = PlaybackStage.IDLE;
                }
                else
                {
                    previewStage = PlaybackStage.COUNTIN;
                    previewBar = 0;
                }
                break;
            default:
                break;
        }
    }

    void PreviewNote()
    {
        //int line = 0;
        if (bars[previewPlayer].eighth[_timer.previewBeat - 1].contains)
            _spawner.SpawnNote(bars[previewPlayer].eighth[_timer.previewBeat - 1].instrumentID, bpm);
    }

    void UpdatePlayer()
    {
        Debug.Log("next player displayed");
    }

    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (_timer.previewBeat == 1) _spawner.SpawnLines(bpm, 1);
        if (_timer.previewBeat == 3) _spawner.SpawnLines(bpm, 2);
        if (_timer.previewBeat == 5) _spawner.SpawnLines(bpm, 3);
        if (_timer.previewBeat == 7) _spawner.SpawnLines(bpm, 4);
    }

        void LastPlayer()
    {
        // if it was last player
        timelineStage = PlaybackStage.IDLE;
        playingBack = false;

        // figure out what else to do
        Debug.Log("this was the last player");

        _display.SwitchLight(false);
        _display.TurnOffCharacter();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        //GUILayout.Box($"Current timeline stage: {timelineStage} + current preview stage {previewStage}, current player: {timelinePlayer}");
    }
#endif
}




// start Playback

// for every player in the game

// display prompt
// display player icon
// display player name
// turn on light, turn off light etc

// play recording of player
// count in 2 - 3 bars (display) maybe with enum of playback stages IDLE, COUNT-IN, PLAYING BACK --> idle and over
// play bar audio + visuals
// possibly play applause
// go to next player or finish

// if done --> stop music, waiting screen, go to voting stage