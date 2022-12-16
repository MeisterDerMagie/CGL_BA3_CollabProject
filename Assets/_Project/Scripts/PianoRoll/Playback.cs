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

public struct AudioNote
{
    public float timeStamp;
    public int sample;
}

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
    public List<Bar> bars;
    public List<AudioBar> barsAudio;

    private void Start()
    {
        //barsAudio = new List<AudioBar>();
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
    }

    /*
    [ContextMenu("testwrite bar")]
    public void Test()
    {
        List<Note> bar = new List<Note>();
        Bar testBar= new Bar();
        List<Bar> testBars = new List<Bar>();

        Note note = new Note();

        testBar.notes = new List<Note>();
        testBars = new List<Bar>();
        

        for (int x = 0; x < 2; x++)
        {
            Bar bar = new Bar();
            bar.notes = new List<Note>();
            testBars.Add(bar);

            for (int i = 0; i < 8; i++)
            {
                note.contains = true;
                note.s = 0;
                testBars[x].notes.Add(note);
            }
        }

        for (int i = 0; i < 8; i++)
        {
            note.contains = true;
            note.s = 0;

            testBar.notes.Add(note);
        }
    }
    */
}
