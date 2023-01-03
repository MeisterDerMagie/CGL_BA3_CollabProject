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
        for (int i = 0; i < 8; i++)
        {
            // time signature of every eighth in the bar counted from one bar before
            // so duration of an eighth * which eighth in the bar + duration of a bar
            float time = (60f / _pianoRoll.bpm / 2f) * i + (60f / _pianoRoll.bpm * 4f);
            compareTo.Add(time);
        }

        dispersion = (60f / _pianoRoll.bpm / 4f);
    }

    public void MapRecording(RecordingNote note)
    {
        for (int i = 0; i < 8; i++)
        {
            // compare to the timeStamp of a normal eighth +- dispersion value of half the duration of an eighth
            if (note.timeStamp >= compareTo[i] - dispersion && note.timeStamp < compareTo[i] + dispersion)
            {
                Eighth e = new Eighth();
                e.contains = true;
                e.instrumentID = note.soundID;

                _recordInput.recordedBar[i] = e;
                // overwrite values in eight[i]
                //_recordInput.recordedBar[i].contains = true;
                //_recordInput.recordedBar[i].soundID = note.soundID;
            }
        }
    }
}
