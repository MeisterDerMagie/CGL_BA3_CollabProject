using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Piano Roll keeps track of the current beat + time (gets accurate info from FMOD) and tells the NoteSpawner to spawn lines and notes at the right time
/// </summary>

public class PianoRoll : MonoBehaviour
{
    private AudioRoll _audioRoll;
    private BackingTrack _backingTrack;
    [SerializeField] private int resetPrevCounter = 5;
    public float bpm = 110f;

    float length4s; // duration of the quarter notes in s
    public float timer; // keeps track of time passed

    public int previewBeat; // keeps track of which beat we are currently on to preview
    public int timeLineBeat; // keeps track of which beat the location marker is currently on
    public int previewBar; //keeps track which bar the preview is currently in
    public int timeLineBar; // keeps track which bar location marker is currently in

    int totalBeats;
    public float zeitVerzögerung;

    bool musicPlaying;
    bool waitForPlayback;
    bool playingBack;
    bool playWithAudio;

    [SerializeField] private List<Bar> bars;
    private List<AudioBar> barsAudio;

    private NoteSpawner spawner;

    void Start()
    {
        _audioRoll = GetComponentInChildren<AudioRoll>();
        _backingTrack = GetComponentInChildren<BackingTrack>();

        length4s = 60f / bpm;
        timer = 0;
        previewBeat = resetPrevCounter;
        timeLineBeat = 0;

        bars = new List<Bar>();
        barsAudio = new List<AudioBar>();
        
        spawner = GetComponent<NoteSpawner>();
        totalBeats = 1;

        _backingTrack.beatUpdated += NextBeat;
        _backingTrack.barUpdated += NextBar;
    }

    private void OnDestroy()
    {
        _backingTrack.beatUpdated -= NextBeat;
        _backingTrack.barUpdated -= NextBar;
    }

    void Update()
    {
        // for testing if quarter lines spawn correctly on time; start and stop by pressing space bar; should be triggered from somewhere else obvs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            musicPlaying = !musicPlaying;
            if (musicPlaying)
            {
                spawner.ActivateIdleLines(false, bpm);
                playingBack = false;
                _backingTrack.StartMusic();
            }
            else
            {
                spawner.ActivateIdleLines(true, bpm);
                //_audioRoll.StopPlaying();
                _backingTrack.StopMusic();

                previewBeat = resetPrevCounter;
                timeLineBeat = 0;
                totalBeats = 1;
            }
        }

        /*
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

                previewBeat++;
                if (previewBeat == 9)
                {
                    previewBeat = 1;
                    previewBar++;
                }
                timeLineBeat++;
                if(timeLineBeat == 9)
                {
                    timeLineBeat = 1;
                    timeLineBar++;
                }

                // check if bars should be counted in
                if (waitForPlayback)
                {
                    if (previewBeat == resetPrevCounter)
                    {
                        playingBack = true;
                        waitForPlayback = false;
                        timeLineBar = -1;
                        previewBar = 0;
                        playWithAudio = true;
                    }
                }

                if (playingBack) PlayBars();
                if (playWithAudio) PlaybackBarAudio();

                PlayQuarterNote();
            }
        }
        else
        {
            timer = 0;
            previewBeat = resetPrevCounter;
            timeLineBeat = 1;
            totalBeats = 1;
        }
        */
    }

    // called from FMOD events via BackingTrack script
    void NextBeat()
    {
        timeLineBeat++;
        if (timeLineBeat == 9)
        {
            timeLineBeat = 1;
            timeLineBar++;
        }
        previewBeat++;
        if (previewBeat == 9)
        {
            previewBeat = 1;
            previewBar++;
        }

        if (waitForPlayback)
        {
            if (previewBeat == 1)
            {
                playingBack = true;
                waitForPlayback = false;
                timeLineBar = -1;
                previewBar = 1;
                playWithAudio = true;
            }
        }

        if (!musicPlaying) return;
        PlayQuarterNote();

        if (playingBack) PlayBars();
        if (playWithAudio) PlaybackBarAudio();
    }

    // called from FMOD events via BackingTrack script
    void NextBar()
    {
    }

    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        //if (previewBeat == 1 || previewBeat == 5) spawner.SpawnLines(bpm);
        if (previewBeat == 1) spawner.SpawnLines(bpm, 1);
        if (previewBeat == 3) spawner.SpawnLines(bpm, 2);
        if (previewBeat == 5) spawner.SpawnLines(bpm, 3);
        if (previewBeat == 7) spawner.SpawnLines(bpm, 4);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.Box($"timeline beat = {timeLineBeat}, preview beat = {previewBeat}");
    }
#endif

    void PlayBars()
    {
        // if the prevbar counter is still one bar too early, don't start yet
        if (previewBar < 1) return;
        // likewise, if the prevbar counter is beyond the limit of the list, stop playing back bars
        // since we start playing back bars on prevBarCounter 1 --> don't have to do >= bars.Count
        if (previewBar > bars.Count)
        {
            playingBack = false;
            return;
        }

        // play preview notes if there is a note on the eighth:
        if (bars[previewBar - 1].eighth[previewBeat - 1].contains)
            spawner.SpawnNote(bars[previewBar - 1].eighth[previewBeat - 1].soundID, bpm);
    }

    void PlaybackBarAudio()
    {
        //if (locBarCounter < bars.Count) return;
        if (timeLineBar < 1) return;

        if (timeLineBar > bars.Count)
        {
            playWithAudio = false;
            return;
        }

        // trigger Audio
        if (bars[timeLineBar - 1].eighth[timeLineBeat - 1].contains)
            _audioRoll.PlaySound(bars[timeLineBar - 1].eighth[timeLineBeat - 1].soundID);
    }
    

    public void StartPlayback(List<Bar> _bars)
    {
        waitForPlayback = true;
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
