using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Piano Roll Timer subscribes to get the updated Beat and Bar Timeline Info via the BackingTrack script from FMOD
/// PianoRollTimer then just counts the beats and bars for use in the Piano Roll
/// </summary>

public enum CurrentGameStage
{
    Recording,
    Playback,
    Voting,
    Repeat
}

public class PianoRollTimer : MonoBehaviour
{
    public int previewBeat; // keeps track of which beat we are currently on to preview
    public int timelineBeat; // keeps track of which beat the location marker is currently on
    public int previewBar; // keeps track which bar the preview is currently in
    public int timelineBar; // keeps track which bar location marker is currently in

    private BackingTrack _backingTrack;

    [SerializeField]
    private CurrentGameStage stage;

    private PianoRollPlayback _playback;
    private PianoRollRecording _recording;
    private PlaybackVoting _voting;
    private PianoRollTLKoffer _tlKoffer;
    private PianoRollPrevKoffer _prevKoffer;

    [SerializeField] CSWriterKoffer csWriter;

    void Start()
    {
        _backingTrack = GetComponentInChildren<BackingTrack>();

        // add self to event delegate for beat updates
        _backingTrack.beatUpdated += NextBeat;

        //timelineBeat = 0;
        //previewBeat = resetPrevCounter;

        switch(stage)
        {
            case CurrentGameStage.Recording:
                _recording = GetComponent<PianoRollRecording>();
                break;
            case CurrentGameStage.Playback:
                _playback = GetComponent<PianoRollPlayback>();
                break;
            case CurrentGameStage.Voting:
                _voting = GetComponent<PlaybackVoting>();
                break;
            case CurrentGameStage.Repeat:
                _tlKoffer = GetComponent<PianoRollTLKoffer>();
                _prevKoffer = GetComponent<PianoRollPrevKoffer>();
                break;
        }
    }

    private void OnDestroy()
    {
        _backingTrack.beatUpdated -= NextBeat;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        //GUILayout.Box($"timeline beat = {timelineBeat}, preview beat = {previewBeat}");
    }
#endif

    public void ResetTimer()
    {
        //previewBeat = resetPrevCounter - 1;
        //timelineBeat = -1 ;
    }

    // called from FMOD events via BackingTrack script
    void NextBeat()
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        timelineBeat = _backingTrack.lastBeat;

            if (timelineBeat == 1)
            {
                previewBeat = 5;
                timelineBar++;
            }
            else if (timelineBeat == 2)
                previewBeat = 6;
            else if (timelineBeat == 3)
                previewBeat = 7;
            else if (timelineBeat == 4)
                previewBeat = 8;
            else if (timelineBeat == 5)
            {
                previewBar++;
                previewBeat = 1;
            }
            else if (timelineBeat == 6)
                previewBeat = 2;
            else if (timelineBeat == 7)
                previewBeat = 3;
            else if (timelineBeat == 8)
                previewBeat = 4;

        // depending on current scene --> set stage in inspector --> tell corresponding piano roll script to update to next beat
        switch (stage)
        {
            case CurrentGameStage.Recording:
                _recording.NextBeat();
                break;
            case CurrentGameStage.Playback:
                _playback.NextBeat();
                break;
            case CurrentGameStage.Voting:
                _voting.NextBeat();
                break;
            case CurrentGameStage.Repeat:
                _tlKoffer.NextBeat();
                _prevKoffer.NextBeat();
                break;
        }

        // for latency scoring:
        /*
        eighth++;
        float targetDate = 0 + eighth * ((60f / 110f) / 2f);
        if (csWriter != null) csWriter.AddNewFmod(targetDate, targetDate, _backingTrack.timeSinceStart, _backingTrack.timeSinceStartUnity);
        */
    }
}
