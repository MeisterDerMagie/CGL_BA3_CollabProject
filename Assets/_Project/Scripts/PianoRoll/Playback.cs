using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data for all bars the players play
[System.Serializable]
public struct Bar
{
    public List<Note> notes;
    // 0 --> 1
    // 1 --> 1 und
    // 2 --> 2
    // 3 --> 2 und
    // 4 --> 3
    // 5 --> 3 und
    // 6 --> 4
    // 7 --> 4 und
}

[System.Serializable]
public struct Note
{
    public bool contains;
    public int s;
}

public class Playback : MonoBehaviour
{
    public List<Bar> bars = new List<Bar>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) PlaybackBars();
    }

    public void AddNewBar(Bar newBar)
    {
        bars.Add(newBar);
    }

    public void RemoveBar(Bar _bar)
    {
        bars.Remove(_bar);
    }

    public void PlaybackBars()
    {

    }
}
