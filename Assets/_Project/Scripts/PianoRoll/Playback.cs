using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Eighth
{
    public bool contains;
    public int soundID;
}

[System.Serializable]
public struct Bar
{
    public List<Eighth> eighth;

    public Bar(List<Eighth> _n)
    {
        eighth = new List<Eighth>();
        eighth = _n;
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
        //_audioRoll.ReceiveAudioBars(bars);
    }
}
