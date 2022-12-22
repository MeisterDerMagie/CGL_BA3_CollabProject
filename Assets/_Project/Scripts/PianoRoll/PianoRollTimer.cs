using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Piano Roll Timer subscribes to get the updated Beat and Bar Timeline Info via the BackingTrack script from FMOD
/// PianoRollTimer then just counts the beats and bars for use in the Piano Roll (and to be reset to the piano roll when different playback situations occur
/// </summary>


public class PianoRollTimer : MonoBehaviour
{
    public int previewBeat; // keeps track of which beat we are currently on to preview
    public int timelineBeat; // keeps track of which beat the location marker is currently on
    public int previewBar; // keeps track which bar the preview is currently in
    public int timeLineBar; // keeps track which bar location marker is currently in

    private BackingTrack _backingTrack;
    [SerializeField] private int resetPrevCounter = 3;


    void Start()
    {
        _backingTrack = GetComponentInChildren<BackingTrack>();

        _backingTrack.beatUpdated += NextBeat;
        _backingTrack.barUpdated += NextBar;

        ResetTimer();
    }

    private void OnDestroy()
    {
        _backingTrack.beatUpdated -= NextBeat;
        _backingTrack.barUpdated -= NextBar;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.Box($"timeline beat = {timelineBeat}, preview beat = {previewBeat}");
    }
#endif

    public void ResetTimer()
    {
        previewBeat = resetPrevCounter;
        timelineBeat = 0;
    }

    // called from FMOD events via BackingTrack script
    void NextBeat()
    {
        timelineBeat++;
        if (timelineBeat == 9)
        {
            timelineBeat = 1;
            timeLineBar++;
        }
        previewBeat++;
        if (previewBeat == 9)
        {
            previewBeat = 1;
            previewBar++;
        }

        GetComponent<PianoRoll>().NextBeat();
    }

    // called from FMOD events via BackingTrack script
    void NextBar()
    {
    }
}
