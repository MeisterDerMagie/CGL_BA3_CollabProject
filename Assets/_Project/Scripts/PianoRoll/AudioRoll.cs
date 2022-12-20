using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioRoll : MonoBehaviour
{
    [SerializeField] private EventReference[] sound;

    [SerializeField] private PianoRoll _pianoRoll;

    bool playing;
    public int currentBar;
    public int currentNote;
    float startTime;

    [SerializeField] private List<AudioBar> audioBars;

    private void Start()
    {
        audioBars = new List<AudioBar>();
        currentBar = 0;
    }

    private void Update()
    {
        //if (!playing) return;

        // if we are in the next bar
        // this may need to be replaced once i set up timeline callbacks from fmod
        /*
        if (_pianoRoll.timer >= (60f / _pianoRoll.bpm) * 4 * (currentBar + 1))
        {
            currentBar++;
            currentNote = 0;
        }
        */
        /*
        if (currentBar > audioBars.Count) return;

        // check if notes are to be played in this bar
        if (audioBars[currentBar].notes.Count != 0)
        {
            // if time passed is greater than (length of a bar * currentBar) + timeStamp of current Note
            if (currentNote > audioBars[currentBar].notes.Count) return;
            if (_pianoRoll.timer >= audioBars[currentBar].notes[currentNote].timeStamp)
            {
                // play current note and increase counter
                PlaySound(audioBars[currentBar].notes[currentNote].sample);
                currentNote++;
            }
        }
        */
    }

    /*
    public void StartPlaying(int current)
    {
        playing = true;
        currentBar = current;
        startTime = _pianoRoll.timer;
    }

    public void StopPlaying()
    {
        playing = false;
        currentBar = 0;
    }

    public void ReceiveAudioBars(List<Bar> bars)
    {
        audioBars.Clear();

        // for every bar that is sent over transfer to an audio bar
        for (int i = 0; i < bars.Count; i++)
        {
            // create a new bar and initialise the list of notes
            AudioBar nb = new AudioBar();
            nb.notes = new List<AudioNote>();

            // create a note for every note in the bar that contains
            for (int a = 0; a < bars[i].notes.Count; a++)
            {
                if (bars[i].notes[a].contains)
                {
                    AudioNote n = new AudioNote();
                    // time stamp is the duration of an eighth * which eighth in the bar
                    n.timeStamp = (60f / _pianoRoll.bpm / 2f) * (a);
                    n.sample = bars[i].notes[a].s;
                    nb.notes.Add(n);
                }
            }

            // add new bar to the list of bars waiting for playback:
            audioBars.Add(nb);
        }
    }
    */

    public void PlaySound(int sample)
    {
        RuntimeManager.PlayOneShot(sound[0]);
    }
}
