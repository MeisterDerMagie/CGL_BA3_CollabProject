using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Piano Roll Spawns the Quarternote Lines and all Notes in the recorded bars of music in time with the backing track
/// </summary>

public class PianoRoll : MonoBehaviour
{
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
    public int beatCounter; // keeps track of which beat we are currently on
    bool playing;
    int beats; // how often are lines spawned, every beat or if 2 every other beat

    private List<Bar> playbackBars;

    //idle lines when piano roll is not playing
    private List<Transform> idleLines;
    private List<GameObject> currentNotes;

    void Start()
    {
        // get the right corner of the piano roll (where objects will be spawned)
        xPos = transform.position.x + bg.transform.localScale.x / 2f;
        length4s = 60f / bpm;
        timer = 0;
        beatCounter = 1;
        currentNotes = new List<GameObject>();

        #region Calculate Positions for all lines
        posLines = new List<float>();
        beats = 2;

        float x = transform.position.x - bg.transform.localScale.x / 2f;

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
        idleLines[0].position = new Vector3(-xPos, 0, 0);

        // set up all other lines
        for (int i = 1; i < idleLines.Count; i++)
        {
            idleLines[i].gameObject.SetActive(true);
            idleLines[i].position = new Vector3(posLines[i-1], 0, 0); 
        }
        #endregion
    }


    void Update()
    {
        // for testing if quarter lines spawn correctly on time; start and stop by pressing space bar; should be triggered by FMOD instead
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playing = !playing;
            if (playing)
            {
                ActivateIdleLines(false);
            }
            else
            {
                ActivateIdleLines(true);
            }
        }


        if (playing)
        {
            // increase timer by amount of ms passed between frames
            timer += Time.deltaTime;

            // keep track of current beat in eights:
            // 60f/bpm is the duration of one beat in s, divide by 2 for eights
            // we multiply by the beatCounter to get the accurate time of the next beat at which to play the quarternote line
            if (timer >= 60f / bpm / 2f * beatCounter)
            {
                beatCounter++;
                if (beatCounter == 9)
                {
                    beatCounter = 1;
                    timer = 0;
                }

                PlayLines();
            }
        }
        else
        {
            timer = 0;
            beatCounter = 1;
        }
    }

    void PlayLines()
    {
        // only spawn on 1s and 3s, so on first and fifth eighth
        if (beatCounter == 1 || beatCounter == 5) SpawnLines();
    }

    void SpawnLines()
    {
        //intantiate a new quarternote line and set position to the far right of the piano roll:
        GameObject clone = Instantiate(beatObj, this.gameObject.transform);
        clone.transform.position = new Vector3(xPos, 0, 0);

        // tell the clone the current bpm, the length of the piano roll in beats, and the target value of position.x (how far it needs to travel to the left)
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.position.x - bg.transform.localScale.x / 2f, this);

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
                clone.transform.position = new Vector3(posLines[i], 0, 0);

                // tell the clone the current bpm, length of piano roll in beats, and target value of poision x (how far it needs to travel to the left)
                clone.GetComponent<Notes>().NoteSetUp(bpm, (i + 1) * beats, transform.position.x - bg.transform.localScale.x / 2f, this);

                currentNotes.Add(clone);
            }
        }
    }

    public void PlayBackBars()
    {

    }





    //----------- not yet up to date:


    void SpawnNote()
    {
        GameObject clone = Instantiate(noteObj, this.gameObject.transform);
        // missing: set correct y Pos according to line
        clone.transform.position = new Vector3(xPos, 0, 0);
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.position.x - bg.transform.localScale.x / 2f, this);

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
