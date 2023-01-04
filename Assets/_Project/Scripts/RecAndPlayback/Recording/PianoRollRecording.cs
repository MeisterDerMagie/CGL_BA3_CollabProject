using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the Piano Roll Script for the recording stage
/// The Piano Roll gets the current beat and bar from the PianoRollTimer
/// it then checks for the preview of the notes if any should be spawned + the lines as well
/// And tells the spawner to spawn them and which one
/// It also tells the AudioRoll script to play the audio at the correct position in the bar
/// </summary>

public class PianoRollRecording : MonoBehaviour
{
    private BackingTrack _backingTrack;
    private AudioRoll _audioRoll;
    private PianoRollTimer _timer;
    private RecordInput _recordInput;
    
    public float bpm = 110f;

    bool musicPlaying;
    private RecPBStage stage;
    private bool playback;
    private bool withAudio;
    private int bar;

    [SerializeField] private List<Bar> bars;

    private NoteSpawner _spawner;

    public enum RecPBStage
    {
        INACTIVE,
        ONLYLINES,
        WAITFORPB,
        PBNOAUDIO,
        PBWAITAUDIO,
        PBWITHAUDIO
    }
    

    void Start()
    {
        _audioRoll = GetComponentInChildren<AudioRoll>();
        _backingTrack = GetComponentInChildren<BackingTrack>();
        _timer = GetComponent<PianoRollTimer>();
        _recordInput = GetComponentInChildren<RecordInput>();

        bars = new List<Bar>();
        
        _spawner = GetComponent<NoteSpawner>();

        // Set button icons
        GetComponentInChildren<PianoRollIcons>().SetUpIcons();

        // Start Backing Track:
        StartMusic();

        // elfenbeinstein MISSING get bpm from FMOD
    }

    void StartMusic()
    {
        musicPlaying = true;
        _spawner.ActivateIdleLines(false, bpm);
        _backingTrack.StartMusic();

        stage = RecPBStage.INACTIVE;
        playback = false;
        withAudio = false;
    }

    public void StopMusic()
    {
        _spawner.ActivateIdleLines(true, bpm);
        _backingTrack.StopMusic();
        _recordInput.StopRecording();
        _timer.ResetTimer();
        musicPlaying = false;

        stage = RecPBStage.INACTIVE;
        playback = false;
        withAudio = false;
    }

    public void NextBeat()
    {
        if (!musicPlaying) return;

        PlaybackLines();

        // if we're waiting to start next preview notes on the one --> start playback and wait for audio
        if (stage == RecPBStage.WAITFORPB && _timer.previewBeat == 1)
        {
            playback = true;
            stage = RecPBStage.PBWAITAUDIO;
            bar = _timer.timelineBar;
        }
        // if we're waiting for audio (so preview note to get to loc marker) --> if we're two bars further and the current beat is one play audio
        else if (stage == RecPBStage.PBWAITAUDIO && _timer.timelineBeat == 1 && _timer.timelineBar - bar == 2)
        {
            withAudio = true;
            stage = RecPBStage.PBWITHAUDIO;
        }

        if (playback) PlaybackNotes();

        if (withAudio) PlaybackAudio();
    }


    void PlaybackLines()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (_timer.previewBeat == 1) _spawner.SpawnLines(bpm, 1);
        if (_timer.previewBeat == 3) _spawner.SpawnLines(bpm, 2);
        if (_timer.previewBeat == 5) _spawner.SpawnLines(bpm, 3);
        if (_timer.previewBeat == 7) _spawner.SpawnLines(bpm, 4);
    }

    void PlaybackNotes()
    {
        int line = 0;

        if (_recordInput.recordedBar[_timer.previewBeat - 1].contains)
        {
            for (int i = 0; i < PlayerData.LocalPlayerData.InstrumentIds.Count; i++)
            {
                if (_recordInput.recordedBar[_timer.previewBeat - 1].instrumentID == PlayerData.LocalPlayerData.InstrumentIds[i])
                    line = i;
            }
            _spawner.SpawnNote(line, bpm);

        }
    }

    void PlaybackAudio()
    {
        if (_recordInput.recordedBar[_timer.timelineBeat - 1].contains)
            _audioRoll.PlaySound(_recordInput.recordedBar[_timer.timelineBeat - 1].instrumentID);
    }

    public void StartPlayback(RecPBStage _recStage)
    {
        switch (_recStage)
        {
            case RecPBStage.INACTIVE:
                playback = false;
                withAudio = false;
                _spawner.ActivateLines(false);
                _spawner.ActivateNotes(false);
                _spawner.spawnActive = false;
                break;
            case RecPBStage.ONLYLINES:
                playback = false;
                withAudio = false;
                _spawner.ActivateLines(true);
                _spawner.ActivateNotes(false);
                _spawner.spawnActive = true;
                break;
            case RecPBStage.PBNOAUDIO:
                playback = true;
                withAudio = false;
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                break;
            case RecPBStage.PBWAITAUDIO:
                playback = true;
                withAudio = false;
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                break;
            case RecPBStage.PBWITHAUDIO:
                playback = true;
                withAudio = true;
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                break;
            case RecPBStage.WAITFORPB:
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                break;
        }

        stage = _recStage;
    }
}
