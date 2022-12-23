using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Piano Roll gets the current beat and bar from the PianoRollTimer
/// it then checks for the preview of the notes if any should be spawned + the lines as well
/// And tells the spawner to spawn them and which one
/// It also tells the AudioRoll script to play the audio at the correct position in the bar
/// </summary>

public class PianoRoll : MonoBehaviour
{
    private BackingTrack _backingTrack;
    private AudioRoll _audioRoll;
    private PianoRollTimer _timer;
    
    public float bpm = 110f;

    bool musicPlaying;
    bool waitForPlayback;
    bool playingBack;
    bool playWithAudio;

    [SerializeField] private List<Bar> bars;

    private NoteSpawner spawner;

    void Start()
    {
        _audioRoll = GetComponentInChildren<AudioRoll>();
        _backingTrack = GetComponentInChildren<BackingTrack>();
        _timer = GetComponent<PianoRollTimer>();

        bars = new List<Bar>();
        
        spawner = GetComponent<NoteSpawner>();
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
                playWithAudio = false;
                playingBack = false;
                _backingTrack.StartMusic();
            }
            else
            {
                spawner.ActivateIdleLines(true, bpm);
                _backingTrack.StopMusic();

                _timer.ResetTimer();
            }
        }
    }

    public void NextBeat()
    {
        if (waitForPlayback)
        {
            if (_timer.previewBeat == 1)
            {
                playingBack = true;
                waitForPlayback = false;
                _timer.timelineBar = -1;
                _timer.previewBar = 1;
                playWithAudio = true;
            }
        }

        if (!musicPlaying) return;
        PlayQuarterNote();

        if (playingBack) PlayBars();
        if (playWithAudio) PlaybackBarAudio();
    }


    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        //if (previewBeat == 1 || previewBeat == 5) spawner.SpawnLines(bpm);
        if (_timer.previewBeat == 1) spawner.SpawnLines(bpm, 1);
        if (_timer.previewBeat == 3) spawner.SpawnLines(bpm, 2);
        if (_timer.previewBeat == 5) spawner.SpawnLines(bpm, 3);
        if (_timer.previewBeat == 7) spawner.SpawnLines(bpm, 4);
    }

    void PlayBars()
    {
        // if the prevbar counter is beyond the limit of the list, stop playing back bars
        // since we start playing back bars on prevBarCounter 1 --> don't have to do >= bars.Count
        if (_timer.previewBar > bars.Count)
        {
            playingBack = false;
            return;
        }

        // play preview notes if there is a note on the eighth:
        if (bars[_timer.previewBar - 1].eighth[_timer.previewBeat - 1].contains)
            spawner.SpawnNote(bars[_timer.previewBar - 1].eighth[_timer.previewBeat - 1].soundID, bpm);
    }

    void PlaybackBarAudio()
    {
        //if (locBarCounter < bars.Count) return;
        if (_timer.timelineBar < 1) return;

        if (_timer.timelineBar > bars.Count)
        {
            playWithAudio = false;
            return;
        }

        // trigger Audio
        if (bars[_timer.timelineBar - 1].eighth[_timer.timelineBeat - 1].contains)
            _audioRoll.PlaySound(bars[_timer.timelineBar - 1].eighth[_timer.timelineBeat - 1].soundID);
    }
    

    public void StartPlayback(List<Bar> _bars)
    {
        waitForPlayback = true;
        playWithAudio = false;
        playingBack = false;
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
