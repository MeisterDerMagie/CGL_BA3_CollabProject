using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Note
{
    public bool contains;
    public int s;
}

[System.Serializable]
public struct Bar
{
    public List<Note> notes;

    public Bar(List<Note> _n)
    {
        notes = new List<Note>();
        notes = _n;
    }

}

[System.Serializable]
public struct AudioNote
{
    public float timeStamp;
    public int sample;
}

[System.Serializable]
public struct AudioBar
{
    public List<AudioNote> notes;

    public AudioBar(List<AudioNote> _n)
    {
        notes = new List<AudioNote>();
        notes = _n;
    }
}

public class Playback : MonoBehaviour
{
    private PianoRoll _pianoRoll;
    [SerializeField] private AudioRoll _audioRoll;
    public List<Bar> bars;
    public List<AudioBar> barsAudio;

    private void Start()
    {
        _pianoRoll = GetComponent<PianoRoll>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) PlaybackBars();
    }

    public void AddNewBar()
    {

    }

    public void RemoveBar()
    {
    }

    public void PlaybackBars()
    {
        _pianoRoll.StartPlayback(bars);
        _audioRoll.ReceiveAudioBars(bars);
    }
}
