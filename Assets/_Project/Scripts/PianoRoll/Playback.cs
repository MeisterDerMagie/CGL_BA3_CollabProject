using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Eighth
{
    public bool contains;
    public int instrumentID;
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

public class Playback : MonoBehaviour
{
    private PianoRoll _pianoRoll;
    private AudioRoll _audioRoll;
    public List<Bar> bars;

    private void Start()
    {
        _pianoRoll = GetComponent<PianoRoll>();
        _audioRoll = GetComponentInChildren<AudioRoll>();
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
    }
}
