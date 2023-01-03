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
    public int timelineBar; // keeps track which bar location marker is currently in

    private BackingTrack _backingTrack;
    [SerializeField] private int resetPrevCounter = 3;


    void Start()
    {
        _backingTrack = GetComponentInChildren<BackingTrack>();

        _backingTrack.beatUpdated += NextBeat;

        timelineBeat = 0;
        previewBeat = resetPrevCounter;
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
        previewBeat = resetPrevCounter - 1;
        timelineBeat = -1 ;
    }

    // called from FMOD events via BackingTrack script
    void NextBeat()
    {
        timelineBeat = _backingTrack.lastBeat;

        if (timelineBeat == 1)
        {
            previewBeat = 4;
            timelineBar++;
        }
        else if (timelineBeat == 2)
            previewBeat = 5;
        else if (timelineBeat == 3)
            previewBeat = 6;
        else if (timelineBeat == 4)
            previewBeat = 7;
        else if (timelineBeat == 5)
            previewBeat = 8;
        else if (timelineBeat == 6)
        {
            previewBeat = 1;
            previewBar++;
        }
        else if (timelineBeat == 7)
            previewBeat = 2;
        else if (timelineBeat == 8)
            previewBeat = 3;

            GetComponent<PianoRollRecording>().NextBeat();
    }
}
