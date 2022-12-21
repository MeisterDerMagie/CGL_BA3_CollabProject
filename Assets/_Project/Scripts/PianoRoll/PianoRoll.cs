using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Piano Roll keeps track of the current beat + time (gets accurate info from FMOD) and tells the NoteSpawner to spawn lines and notes at the right time
/// </summary>

public class PianoRoll : MonoBehaviour
{
    [SerializeField] private AudioRoll _audioRoll;
    [SerializeField] private int resetPrevCounter = 5;
    public float bpm = 110f;

    float length4s; // duration of the quarter notes in s
    public float timer; // keeps track of time passed

    public int prevBeatCounter; // keeps track of which beat we are currently on to preview
    public int locBeatCounter; // keeps track of which beat the location marker is currently on
    public int prevBarCounter; //keeps track which bar the preview is currently in
    public int locBarCounter; // keeps track which bar location marker is currently in

    int totalBeats;
    public float zeitVerzögerung;

    bool musicPlaying;
    bool waitForBars;
    bool barsPlaying;
    bool playAudio;

    [SerializeField] private List<Bar> bars;
    private List<AudioBar> barsAudio;

    private NoteSpawner spawner;

    void Start()
    {
        length4s = 60f / bpm;
        timer = 0;
        prevBeatCounter = resetPrevCounter;
        locBeatCounter = 1;

        bars = new List<Bar>();
        barsAudio = new List<AudioBar>();
        
        spawner = GetComponent<NoteSpawner>();
        totalBeats = 1;
    }


    void Update()
    {
        // for testing if quarter lines spawn correctly on time; start and stop by pressing space bar; should be triggered by FMOD instead
        if (Input.GetKeyDown(KeyCode.Space))
        {
            musicPlaying = !musicPlaying;
            if (musicPlaying)
            {
                spawner.ActivateIdleLines(false, bpm);
                barsPlaying = false;
            }
            else
            {
                spawner.ActivateIdleLines(true, bpm);
                //_audioRoll.StopPlaying();
            }
        }


        if (musicPlaying)
        {
            // increase timer by amount of ms passed between frames
            timer += Time.deltaTime;

            // keep track of current beat in eights:
            // 60f/bpm is the duration of one beat in s, divide by 2 for eights
            // we multiply by the beatCounter to get the accurate time of the next beat at which to play the quarternote line
            if (timer >= (60f / bpm / 2f * totalBeats) - zeitVerzögerung)
            {
                totalBeats++;

                prevBeatCounter++;
                if (prevBeatCounter == 9)
                {
                    prevBeatCounter = 1;
                    prevBarCounter++;
                }
                locBeatCounter++;
                if(locBeatCounter == 9)
                {
                    locBeatCounter = 1;
                    locBarCounter++;
                    /*
                    if (playAudio) 
                    {
                        if (locBarCounter == 1) _audioRoll.StartPlaying(0);
                        else
                        {
                            _audioRoll.currentBar++;
                            _audioRoll.currentNote = 0;
                        }
                    }
                    */
                }

                // check if bars should be counted in
                if (waitForBars)
                {
                    if (prevBeatCounter == resetPrevCounter)
                    {
                        barsPlaying = true;
                        waitForBars = false;
                        locBarCounter = -1;
                        prevBarCounter = 0;
                        playAudio = true;
                    }
                }

                if (barsPlaying) PlayBars();
                if (playAudio) PlaybackBarAudio();

                PlayQuarterNote();
            }
        }
        else
        {
            timer = 0;
            prevBeatCounter = resetPrevCounter;
            locBeatCounter = 1;
            totalBeats = 1;
        }
    }

    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (prevBeatCounter == 1 || prevBeatCounter == 5) spawner.SpawnLines(bpm);
    }

    void PlayBars()
    {
        // if the prevbar counter is still one bar too early, don't start yet
        if (prevBarCounter == 0) return;
        // likewise, if the prevbar counter is beyond the limit of the list, stop playing back bars
        // since we start playing back bars on prevBarCounter 1 --> don't have to do >= bars.Count
        if (prevBarCounter > bars.Count)
        {
            barsPlaying = false;
            playAudio = true;
            return;
        }

        // play preview notes if there is a note on the eighth:
        if (bars[prevBarCounter - 1].eighth[prevBeatCounter - 1].contains)
            spawner.SpawnNote(bars[prevBarCounter - 1].eighth[prevBeatCounter - 1].soundID, bpm);
    }

    void PlaybackBarAudio()
    {
        //if (locBarCounter < bars.Count) return;
        if (locBarCounter < 1) return;

        if (locBarCounter > bars.Count)
        {
            playAudio = false;
            return;
        }

        // trigger Audio
        if (bars[locBarCounter - 1].eighth[locBeatCounter - 1].contains)
            _audioRoll.PlaySound(bars[locBarCounter - 1].eighth[locBeatCounter - 1].soundID);
    }
    

    public void StartPlayback(List<Bar> _bars)
    {
        waitForBars = true;
        bars.Clear();
        
        // for every bar that is sent over
        for (int i = 0; i < _bars.Count; i++)
        {
            // create a new bar and initialise the list of notes
            Bar nb = new Bar();
            nb.eighth = new List<Eighth>();

            // create a note and add values from the note sent over for every note in that bar
            for (int a = 0; a < _bars[i].eighth.Count; a++)
            {
                Eighth n = new Eighth();
                n.contains = _bars[i].eighth[a].contains;
                n.soundID = _bars[i].eighth[a].soundID;
                nb.eighth.Add(n);
            }

            // add new bar to the list of bars waiting to be played back
            bars.Add(nb);
        }
    }
}
