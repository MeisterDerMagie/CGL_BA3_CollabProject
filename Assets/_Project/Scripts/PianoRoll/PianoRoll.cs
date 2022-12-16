using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Piano Roll Spawns the Quarternote Lines and all Notes in the recorded bars of music in time with the backing track
/// </summary>

public class PianoRoll : MonoBehaviour
{
    [SerializeField] private int resetPrevCounter = 7;
    [SerializeField] private float bpm = 110f;
    [Tooltip("Amount of lines in piano roll, so how many different kinds of notes spawned on top of each other")]
    [SerializeField] private int lineHeight = 4;
    [Tooltip("Length of Piano Roll in beats, how many beats fit into piano roll")]
    [SerializeField] private int beatLength = 8;
    [Tooltip("how many beats are to the right side of the location marker; how many beats can you preview before they need to be played")]
    [SerializeField] private int previewLength = 6; 

    [SerializeField] private GameObject beatObj; // the quarternote lines marking every beat
    [SerializeField] private GameObject noteObj; // the note spawned
    [SerializeField] private GameObject bg; // the background image of the piano roll, to get correct size

    [SerializeField] List<float> posLines; // where all lines should be spawned idle, where the downbeats are on piano roll
    float xPos; // where new notes should be spawned
    float length4s; // duration of the quarter notes in s
    float timer; // keeps track of time passed
    public int prevBeatCounter; // keeps track of which beat we are currently on to preview
    public int locBeatCounter; // keeps track of which beat the location marker is currently on
    public int prevBarCounter; //keeps track which bar the preview is currently in
    public int locBarCounter; // keeps track which bar location marker is currently in
    bool musicPlaying;
    bool waitForBars;
    bool barsPlaying;
    int beats; // how often are lines spawned, every beat or if 2 every other beat

    //idle lines when piano roll is not playing
    private List<Transform> idleLines;
    private List<GameObject> currentNotes;

    [SerializeField] private List<Bar> bars;
    private List<AudioBar> barsAudio;

    void Start()
    {
        // get the right corner of the piano roll (where objects will be spawned)
        xPos = transform.localPosition.x + bg.transform.localScale.x / 2f;
        length4s = 60f / bpm;
        timer = 0;
        prevBeatCounter = resetPrevCounter;
        locBeatCounter = 1;
        currentNotes = new List<GameObject>();
        bars = new List<Bar>();
        barsAudio = new List<AudioBar>();

        #region Calculate Positions for all lines
        posLines = new List<float>();
        beats = 2;

        float x = transform.localPosition.x - bg.transform.localScale.x / 2f;

        for (int i = 0; i <= beatLength / beats - 1; i++)
        {
            x += beats * bg.transform.localScale.x / beatLength;
            posLines.Add(x);
        }
        #endregion

        #region SetUpIdleLines
        // set up idle lines when the piano roll is not playing:
        idleLines = new List<Transform>();

        // clone lines and add to list of idle lines:
        for (int i = 0; i < beatLength / beats + 1; i++)
        {
            Transform clone = Instantiate(beatObj, this.gameObject.transform).transform;
            idleLines.Add(clone.transform);
        }

        // set up first idle line at left hand corner of piano roll
        idleLines[0].localPosition = new Vector3(-xPos, 0, 0);

        // set up all other lines
        for (int i = 1; i < idleLines.Count; i++)
        {
            idleLines[i].gameObject.SetActive(true);
            idleLines[i].localPosition = new Vector3(posLines[i-1], 0, 0); 
        }
        #endregion
    }


    void Update()
    {
        // for testing if quarter lines spawn correctly on time; start and stop by pressing space bar; should be triggered by FMOD instead
        if (Input.GetKeyDown(KeyCode.Space))
        {
            musicPlaying = !musicPlaying;
            if (musicPlaying)
            {
                ActivateIdleLines(false);
                barsPlaying = false;
            }
            else
            {
                ActivateIdleLines(true);
            }
        }


        if (musicPlaying)
        {
            // increase timer by amount of ms passed between frames
            timer += Time.deltaTime;

            // keep track of current beat in eights:
            // 60f/bpm is the duration of one beat in s, divide by 2 for eights
            // we multiply by the beatCounter to get the accurate time of the next beat at which to play the quarternote line
            if (timer >= 60f / bpm / 2f * locBeatCounter)
            {
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
                    timer = 0;
                }

                // check if bars should be counted in
                if (waitForBars)
                {
                    if (prevBeatCounter == resetPrevCounter) barsPlaying = true;
                    waitForBars = false;
                    locBarCounter = -1;
                    prevBarCounter = 0;
                }

                if (barsPlaying) PlayBars();
                PlayLines();
            }
        }
        else
        {
            timer = 0;
            prevBeatCounter = resetPrevCounter;
            locBeatCounter = 1;
        }
    }

    void PlayLines()
    {
        // only spawn on 1s and 3s, so on first and fifth eighth
        if (prevBeatCounter == 1 || prevBeatCounter == 5) SpawnLines();
    }

    void SpawnLines()
    {
        //intantiate a new quarternote line and set position to the far right of the piano roll:
        GameObject clone = Instantiate(beatObj, this.gameObject.transform);
        clone.transform.localPosition = new Vector3(xPos, 0, 0);

        // tell the clone the current bpm, the length of the piano roll in beats, and the target value of position.x (how far it needs to travel to the left)
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.localPosition.x - bg.transform.localScale.x / 2f, this);

        currentNotes.Add(clone);
    }

    public void RemoveNote(GameObject obj)
    {
        currentNotes.Remove(obj);
    }

    void ActivateIdleLines(bool var)
    {
        // de/activate all lines in idle lines
        foreach (Transform obj in idleLines)
        {
            obj.gameObject.SetActive(var);
        }

        // if idle lines are activated, remove all other active notes
        // better alternative if time --> let them move until they hit the next beat
        if (var)
        {
            foreach (GameObject obj in currentNotes)
            {
                Destroy(obj);
            }
            currentNotes.Clear();
        }
        else
        {
            // if idle lines are deactivated, spawn new lines that move:
            for (int i = 0; i < posLines.Count; i++)
            {
                // instantiate a new line and set position to what is saved in posLines
                GameObject clone = Instantiate(beatObj, this.gameObject.transform);
                clone.transform.localPosition = new Vector3(posLines[i], 0, 0);

                // tell the clone the current bpm, length of piano roll in beats, and target value of poision x (how far it needs to travel to the left)
                clone.GetComponent<Notes>().NoteSetUp(bpm, (i + 1) * beats, transform.localPosition.x - bg.transform.localScale.x / 2f, this);

                currentNotes.Add(clone);
            }
        }
    }

    public void PlayBars()
    {
        if (prevBarCounter == 0) return;
        if (prevBarCounter >= bars.Count) return;
        
        // play preview notes:
        if (bars[prevBarCounter - 1].notes[prevBeatCounter - 1].contains)
        {
            Debug.Log("note should be spawned");
            SpawnNote(0);
        }
    }

    public void StartPlayback(List<Bar> _bars)
    {
        waitForBars = true;
        //bars.Clear();
        bars = _bars;
        /*
        for (int i = 0; i < _bars.Count; i++)
        {
            Bar nb = new Bar();
            bars.Add(nb);

            for (int a = 0; a < _bars[i].notes.Count; a++)
            {
                Note n = new Note();
                n = _bars[i].notes[a];
                bars[i].notes.Add(n);
            }
        }
        */
    }




    //----------- not yet up to date:


    void SpawnNote(int sample)
    {
        GameObject clone = Instantiate(noteObj, this.gameObject.transform);
        // missing: set correct y Pos according to line
        clone.transform.localPosition = new Vector3(xPos, 0, 0);
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.localPosition.x - bg.transform.localScale.x / 2f, this);

        currentNotes.Add(clone);
    }

    // different way of doing it:
    // spawn one bar at a time
    IEnumerator SpawnQuarterTicks()
    {
        SpawnLines();
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(length4s);
            SpawnLines();
        }
    }
}
