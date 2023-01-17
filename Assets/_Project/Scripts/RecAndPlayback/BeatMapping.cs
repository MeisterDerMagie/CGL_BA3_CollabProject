using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMapping : MonoBehaviour
{
    private RecordInput _recordInput;
    private AccuracyScoring scoring;

    public float dispersion;
    public List<float> compareTo;

    public List<ScoringNote> compareToScoring;

    float bpm;

    // dispersion for:
    public float hit = 0.034f;
    public float almost = 0.103f;
    public float latency;

    public enum ScoringType
    {
        HIT,
        ALMOST,
        MISS
    }

    [System.Serializable]
    public struct ScoringNote
    {
        public float timeStamp;
        public float instrumentID;
        public float playerID;
    }

    private void Start()
    {
        _recordInput = GetComponent<RecordInput>();
    }

    public void SetUp(float _bpm)
    {
        bpm = _bpm;
        dispersion = (60f / bpm / 4f);
    }

    public void PrepareRecording()
    {
        compareTo = new List<float>();
        for (int i = 0; i < 8 * Constants.RECORDING_LENGTH ; i++)
        {
            // time signature of every eighth in the bar times the amount of bars counted from one bar before
            // so duration of an eighth * which eighth in the bar + duration of a bar
            float time = (60f / bpm / 2f) * i + (60f / bpm * 4f);
            compareTo.Add(time);
        }

        // dispersion is the duration of half an eighth
        dispersion = (60f / bpm / 4f);
    }

    public void MapRecording(RecordingNote note)
    {
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
                    if (i < 8 * b) bar = b - 1;
                }
                int eighth = i - bar * 8;
                _recordInput.recording[bar][eighth] = e;
            }
        }
    }

    public void PrepareAccuracyScoring(float _bpm, List<Eighth> _list)
    {
        bpm = _bpm;

        compareToScoring = new List<ScoringNote>();

        /*
        // go through list of eighths (which are all recordings of all players and write new list of floats with time stamps
        for (int i = 0; i < _list.Count; i++)
        {
            // for every eighth that contains a note, add to compare to list
            if (_list[i].contains)
            {
                ScoringNote note = new ScoringNote();

                // timestamp of eighth is: duration of a bar (because timer starts 1 bar before first eighth) PLUS eighth times duration of an eighth
                // duration of a bar: 60/bpm is duration of a quarter note, times 4 is duration of a bar
                // duration of an eighth is 60 / bpm / 2
                note.timeStamp = ((60f / bpm) * 4f) + i * ((60f / bpm) / 2f);
                note.instrumentID = _list[i].instrumentID;
                //note.playerID = 
            }
        }
        */

        for (int p = 0; p < _list.Count / (Constants.RECORDING_LENGTH * 8); p++)
        {
            for (int i = 0; i < Constants.RECORDING_LENGTH * 8; i++)
            {
                ScoringNote note = new ScoringNote();

                // timestamp of eighth is: duration of a bar (because timer starts 1 bar before first eighth) = 60/bpm is duration of a quarter note, times 4 is duration of a bar
                // PLUS duration of a bar * which player it is * recording length
                // PLUS eighth times duration of an eighth = 60 / bpm / 2
                note.timeStamp = ((60f / bpm) * 4f) + p * ((60f / bpm) * 4f) * Constants.RECORDING_LENGTH + i * ((60f / bpm) / 2f);
                note.instrumentID = _list[i].instrumentID;
                note.playerID = p;

                compareToScoring.Add(note);
            }
        }

    }

    public void ScoreAccuracy(RecordingNote note, bool scorePlayability)
    {
        ScoringType type = ScoringType.HIT;

        // MISSING: actually determine if it was hit or miss

        for (int i = 0; i < compareToScoring.Count; i++)
        {

        }

    }
}
