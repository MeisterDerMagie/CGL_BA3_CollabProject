using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMapping : MonoBehaviour
{
    private PianoRollRecording _pianoRoll;
    private RecordInput _recordInput;

    public float dispersion;
    public List<float> compareTo;

    private void Start()
    {
        _pianoRoll = GetComponentInParent<PianoRollRecording>();
        _recordInput = GetComponent<RecordInput>();

        dispersion = (60f / _pianoRoll.bpm / 4f);
    }

    public void PrepareRecording()
    {
        compareTo = new List<float>();
        for (int i = 0; i < 8 * Constants.RECORDING_LENGTH ; i++)
        {
            // time signature of every eighth in the bar times the amount of bars counted from one bar before
            // so duration of an eighth * which eighth in the bar + duration of a bar
            float time = (60f / _pianoRoll.bpm / 2f) * i + (60f / _pianoRoll.bpm * 4f);
            compareTo.Add(time);
        }

        // dispersion is the duration of half an eighth
        dispersion = (60f / _pianoRoll.bpm / 4f);
    }

    public void MapRecording(RecordingNote note)
    {
        /*
        for (int i = 0; i < 8; i++)
        {
            // compare to the timeStamp of a normal eighth +- dispersion value of half the duration of an eighth
            if (note.timeStamp >= compareTo[i] - dispersion && note.timeStamp < compareTo[i] + dispersion)
            {
                Eighth e = new Eighth();
                e.contains = true;
                e.instrumentID = note.soundID;

                // decide which bar to put into
                _recordInput.recordedBar[i] = e;
            }
        }
        */

        for (int i = 0; i < 8 * Constants.RECORDING_LENGTH; i++)
        {
            // compare to the timestamp of the eighth saved +- dispersion value of half the duration of an eighth
            if (note.timeStamp >= compareTo[i] - dispersion && note.timeStamp < compareTo[i] + dispersion)
            {
                Eighth e = new Eighth();
                e.contains = true;
                e.instrumentID = note.soundID;

                // which bar to put into
                int bar = 0;
                for (int b = Constants.RECORDING_LENGTH; b >= 1; b--)
                {
                    if (i <= 8 * (b - 1)) bar = b - 1;
                }
                int eighth = i - bar * 8;
                _recordInput.recording[bar][eighth] = e;
            }
        }
    }
}
