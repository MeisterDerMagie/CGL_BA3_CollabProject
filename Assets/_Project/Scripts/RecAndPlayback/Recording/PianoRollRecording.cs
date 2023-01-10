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

    int previewBar;
    int timelineBar;

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

        _spawner = GetComponent<NoteSpawner>();

        // Start Backing Track:
        StartMusic();
    }

    void StartMusic()
    {
        musicPlaying = true;
        _spawner.ActivateIdleLines(true);
        _spawner.ActivateLines(false);
        _spawner.ActivateNotes(false);
        _backingTrack.StartMusic();

        stage = RecPBStage.INACTIVE;
        playback = false;
        withAudio = false;
    }

    public void StopMusic()
    {
        _spawner.ActivateIdleLines(true);
        _spawner.ActivateLines(false);
        _spawner.ActivateNotes(false);

        _backingTrack.StopMusic();
        _recordInput.StopRecording();
        musicPlaying = false;

        stage = RecPBStage.INACTIVE;
        playback = false;
        withAudio = false;
    }

    public void NextBeat()
    {
        if (!musicPlaying) return;

        if (_timer.previewBeat == 1)
        {
            // if we're waiting to start next preview notes on the one --> start playback and wait for audio
            if (stage == RecPBStage.WAITFORPB)
            {
                playback = true;
                stage = RecPBStage.PBWAITAUDIO;
                bar = _timer.timelineBar;
                previewBar = 0;
            }
            else
            {
                previewBar++;
                if (previewBar >= Constants.RECORDING_LENGTH) previewBar = 0;
            }
        }
        if (_timer.timelineBeat == 1)
        {
            // if we're waiting for audio (so preview note to get to loc marker) --> if we're two bars further and the current beat is one play audio
            if (stage == RecPBStage.PBWAITAUDIO && _timer.timelineBar - bar == 2)
            {
                withAudio = true;
                stage = RecPBStage.PBWITHAUDIO;
                timelineBar = 0;
            }
            else
            {
                timelineBar++;
                if (timelineBar >= Constants.RECORDING_LENGTH) timelineBar = 0;
            }
        }

        PlaybackLines();

        // preview notes
        if (playback)
            if (_recordInput.recording[previewBar][_timer.previewBeat - 1].contains)
                _spawner.SpawnNote(_recordInput.recording[previewBar][_timer.previewBeat - 1].instrumentID, bpm);
        /*
        if (playback)
            if (_recordInput.recordedBar[_timer.previewBeat - 1].contains) 
                _spawner.SpawnNote(_recordInput.recordedBar[_timer.previewBeat - 1].instrumentID, bpm);*/

        // playback audio
        if (withAudio)
            if (_recordInput.recording[timelineBar][_timer.timelineBeat - 1].contains)
                _audioRoll.PlaySound(_recordInput.recording[timelineBar][_timer.timelineBeat - 1].instrumentID);
        /*
        if (withAudio) 
            if (_recordInput.recordedBar[_timer.timelineBeat - 1].contains)
                _audioRoll.PlaySound(_recordInput.recordedBar[_timer.timelineBeat - 1].instrumentID);*/
    }


    void PlaybackLines()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (_timer.previewBeat == 1)
        {
            if (stage == RecPBStage.PBNOAUDIO || stage == RecPBStage.PBWAITAUDIO || stage == RecPBStage.PBWITHAUDIO)
            {

            }
            else _spawner.SpawnLine(bpm, 1);
        }
        if (_timer.previewBeat == 3) _spawner.SpawnLine(bpm, 2);
        if (_timer.previewBeat == 5) _spawner.SpawnLine(bpm, 3);
        if (_timer.previewBeat == 7) _spawner.SpawnLine(bpm, 4);
    }

    public void ControlPlayback(RecPBStage _recStage)
    {
        switch (_recStage)
        {
            case RecPBStage.INACTIVE:
                playback = false;
                withAudio = false;
                _spawner.ActivateLines(false);
                _spawner.ActivateNotes(false);
                _spawner.spawnActive = false;
                _spawner.ActivateIdleLines(true);
                break;
            case RecPBStage.ONLYLINES:
                playback = false;
                withAudio = false;
                _spawner.ActivateLines(true);
                _spawner.ActivateNotes(false);
                _spawner.spawnActive = true;
                _spawner.ActivateIdleLines(false);
                break;
            case RecPBStage.PBNOAUDIO:
                // shouldn't be here
                /*playback = true;
                withAudio = false;
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                _spawner.ActivateIdleLines(false);*/
                break;
            case RecPBStage.PBWAITAUDIO:
                // shouldn't be here
                /*playback = true;
                withAudio = false;
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                _spawner.ActivateIdleLines(false);*/
                break;
            case RecPBStage.PBWITHAUDIO:
                // shouldn't be here
                /*playback = true;
                withAudio = true;
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                _spawner.ActivateIdleLines(false);*/
                break;
            case RecPBStage.WAITFORPB:
                _spawner.ActivateLines(true);
                _spawner.spawnActive = true;
                _spawner.ActivateIdleLines(false);
                break;
        }

        stage = _recStage;
    }
}
