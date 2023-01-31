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
        PLAYING,
        END
    }

    [SerializeField] private CharDisplayPB _display;
    [SerializeField] private PlaybackScenesAudio _playbackAudio;
    private PianoRollTimer _timer;
    private AudioRoll _audioRoll;
    private NoteSpawner _spawner;
    private PianoRollParticles _particles;
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
    [Tooltip ("bars after playback finished before going to next screen")]
    [SerializeField] int fadeOut = 2;

    public List<PlayerData> playerDatas;
    int timelinePlayer;
    int previewPlayer;

    int timelineTimer;
    int startPreviewBar;

    int timelineBar;
    int previewBar;

    // this is more of a joke, but it's the list of players that have a list of bars that contain a list of eighths
    List<List<List<Eighth>>> recordings; 

    private PlaybackStage timelineStage;
    private PlaybackStage previewStage;
    private bool playingBack;

    public override void OnNetworkSpawn()
    {
        // find all players and their data
        playerDatas = FindObjectsOfType<PlayerData>().ToList();
        
        //sort player datas 
        playerDatas = playerDatas.OrderBy(data => data.PlayerName).ToList();
        
        timelinePlayer = 0;
        previewPlayer = 0;

        // again the jokey way of doing it
        recordings = new List<List<List<Eighth>>>();
        // for every player in the list
        for (int player = 0; player < playerDatas.Count; player++)
        {
            List<List<Eighth>> playerRec = new List<List<Eighth>>();
            
            // and every bar in the recordings
            for (int bar = 0; bar < Constants.RECORDING_LENGTH; bar++)
            {
                List<Eighth> _bar = new List<Eighth>();

                // and every eighth in a bar
                for (int eighth = 0; eighth < 8; eighth++)
                {
                    // create an eighth and take values from the player Data list
                    Eighth e = new Eighth();

                    e.contains = playerDatas[player].Recording[eighth + (8 * bar)].contains;
                    e.instrumentID = playerDatas[player].Recording[eighth + (8 * bar)].instrumentID;

                    // add to the bar
                    _bar.Add(e);
                }
                // add the full bar to the player's Recording
                playerRec.Add(_bar);
            }
            // and add the player's recording to the list of all recordings
            recordings.Add(playerRec);
        }

        // set scripts
        _timer = GetComponent<PianoRollTimer>();
        _audioRoll = GetComponentInChildren<AudioRoll>();
        _spawner = GetComponent<NoteSpawner>();
        _particles = GetComponentInChildren<PianoRollParticles>();

        // set up playing back to false --> don't start too soon
        playingBack = false;
        timelineStage = PlaybackStage.IDLE;
        previewStage = PlaybackStage.IDLE;

        // ui stuff
        _light.TurnOff();
        _display.TurnOffCharacter();
        _playbackAudio.PlayDrumRoll();
        _playbackAudio.PlayCrowd();

        // preload instances of player sounds:
        _audioRoll.SetUpAllInstances();

        // wait short time for playback to start
        StartCoroutine(WaitToStart());

        if (Constants.RECORDING_LENGTH == 2 && barsBetween == 3) barsBetween = 2;
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

        timelineTimer = _timer.timelineBar;
        startPreviewBar = _timer.previewBar;

        // set currentPlayer to zero
        timelinePlayer = 0;
        previewPlayer = 0;

        // set current bars to zero (if there's more than one bar recorded)
        timelineBar = 0;
        previewBar = 0;

        // set current stages
        timelineStage = PlaybackStage.COUNTIN;
        previewStage = PlaybackStage.COUNTIN;

        // update display and switch off light
        _light.TurnOff();

        _particles.TurnOnParticle(true);
        _playbackAudio.StopCrowd();
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
        if (NetworkManager.IsServer) return;

        if (timelineStage == PlaybackStage.END)
        {
            if (_timer.timelineBeat == 1)
            {
                timelineBar++;
            }

            if (timelineBar >= fadeOut) End();
            return;
        }

        if (!playingBack) return;
        if (_timer.timelineBeat == 0) return;
        
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

        PlayQuarterNote();

        // update display
        if (previewStage == PlaybackStage.PLAYING)
            if (recordings[previewPlayer][previewBar][_timer.previewBeat - 1].contains)
                _spawner.SpawnNote(recordings[previewPlayer][previewBar][_timer.previewBeat - 1].instrumentID, bpm);

        // play audio
        if (timelineStage == PlaybackStage.PLAYING)
            if (recordings[timelinePlayer][timelineBar][_timer.timelineBeat - 1].contains)
                _audioRoll.PlayerInputSound(PlayerData.LocalPlayerData.InstrumentIds.IndexOf(recordings[timelinePlayer][timelineBar][_timer.timelineBeat - 1].instrumentID));
        
        /*
        // update display
        if (previewStage == PlaybackStage.PLAYING)
            if (playerDatas[previewPlayer].Recording[_timer.previewBeat - 1].contains)
                _spawner.SpawnNote(playerDatas[previewPlayer].Recording[_timer.previewBeat - 1].instrumentID, bpm);

        // play audio
        if (timelineStage == PlaybackStage.PLAYING)
            if (playerDatas[timelinePlayer].Recording[_timer.timelineBeat - 1].contains)
                _audioRoll.PlaySound(playerDatas[timelinePlayer].Recording[_timer.timelineBeat - 1].instrumentID);
        */
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
                    if (_timer.timelineBar - timelineTimer == countIn)
                    {
                        UpdatePlayer();
                    }
                    if (_timer.timelineBar - timelineTimer == countIn + 1)
                    {
                        // turn on light and change state to playing
                        _light.TurnOn();
                        timelineStage = PlaybackStage.PLAYING;
                        timelineTimer = _timer.timelineBar;
                        timelineBar = 0;
                    }
                }
                else
                {
                    if (_timer.timelineBar - timelineTimer == 1)
                    {
                        // Update visuals to new player
                        UpdatePlayer();
                    }

                    if (_timer.timelineBar - timelineTimer == barsBetween)
                    {
                        // turn on light and change state to playing
                        _light.TurnOn();
                        timelineStage = PlaybackStage.PLAYING;
                        timelineTimer = _timer.timelineBar;
                        timelineBar = 0;
                    }
                }
                break;
            case PlaybackStage.PLAYING:
                if (_timer.timelineBar - timelineTimer == Constants.RECORDING_LENGTH)
                {
                    timelinePlayer++;
                    // check if this was last player
                    if (timelinePlayer > playerDatas.Count - 1)
                    {
                        LastPlayer();
                    }
                    else
                    {
                        timelineStage = PlaybackStage.COUNTIN;
                        timelineTimer = _timer.timelineBar;
                        _playbackAudio.PlayApplauseShort();
                    }
                    _light.TurnOff();
                }
                else timelineBar++;
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
                    if (_timer.previewBar - startPreviewBar == countIn - 1)
                    {
                        previewStage = PlaybackStage.PLAYING;
                        startPreviewBar = _timer.previewBar;
                        previewBar = 0;
                    }
                }
                else
                {
                    if (_timer.previewBar - startPreviewBar == barsBetween)
                    {
                        previewStage = PlaybackStage.PLAYING;
                        startPreviewBar = _timer.previewBar;
                        previewBar = 0;
                    }
                }
                break;
            case PlaybackStage.PLAYING:
                if (_timer.previewBar - startPreviewBar == Constants.RECORDING_LENGTH)
                {
                    previewPlayer++;
                    // check if last player
                    if (previewPlayer > playerDatas.Count - 1)
                    {
                        previewStage = PlaybackStage.END;
                        startPreviewBar = _timer.previewBar;
                    }
                    else
                    {
                        previewStage = PlaybackStage.COUNTIN;
                        startPreviewBar = _timer.previewBar;
                    }
                }
                else previewBar++;
                break;
            case PlaybackStage.END:
                previewStage = PlaybackStage.IDLE;
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

        // play audio:
        _playbackAudio.PlayCharSwish();
        _playbackAudio.PlayCatchPhrase(CharacterManager.Instance.GetCharacter(playerDatas[timelinePlayer].CharacterId).catchPhrase);
    }

    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (_timer.previewBeat == 1)
        {
            if (previewStage == PlaybackStage.PLAYING && previewBar == 0)
                _spawner.SpawnLine(bpm, 1, true);
            else if (previewStage == PlaybackStage.COUNTIN && previewPlayer != 0 && _timer.previewBar - startPreviewBar == 0)
                _spawner.SpawnLine(bpm, 1, true);
            else if (previewStage == PlaybackStage.END && _timer.previewBar - startPreviewBar == 0)
                _spawner.SpawnLine(bpm, 1, true);
            else _spawner.SpawnLine(bpm, 1);
        }
        if (_timer.previewBeat == 3) _spawner.SpawnLine(bpm, 2);
        if (_timer.previewBeat == 5) _spawner.SpawnLine(bpm, 3);
        if (_timer.previewBeat == 7) _spawner.SpawnLine(bpm, 4);
    }

        void LastPlayer()
    {
        // if it was last player
        timelineStage = PlaybackStage.END;
        timelineBar = 0;
        playingBack = false;

        _light.TurnOff();
        _display.TurnOffCharacter();
        _playbackAudio.PlayCharSwish();
        _playbackAudio.PlayApplause();
    }

    void End()
    {
        GetComponentInChildren<BackingTrack>().StopMusic();
        _particles.TurnOnParticle(false);
        PersistentAudioManager.Singleton.FadeInAmbience();
        PersistentAudioManager.Singleton.FadeInMainTheme();

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

