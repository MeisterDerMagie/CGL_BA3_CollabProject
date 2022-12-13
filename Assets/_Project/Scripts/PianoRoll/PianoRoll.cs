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
    [SerializeField] private int lines = 4;
    [Tooltip("Length of Piano Roll in beats, how many beats fit into piano roll")]
    [SerializeField] private int beats = 8;

    [SerializeField] private GameObject beatObj; // the quarternote lines marking every beat
    [SerializeField] private GameObject noteObj; // the note spawned
    [SerializeField] private GameObject bg; // the background image of the piano roll, to get correct size

    float xPos; // where new notes should be spawned
    float length4s; // duration of the quarter notes in s
    float timer; // keeps track of time passed
    int beatCounter; // keeps track of which beat we are currently on
    bool playing;

    void Start()
    {
        // get the right corner of the piano roll (where objects will be spawned)
        xPos = transform.position.x + bg.transform.localScale.x / 2f;
        length4s = 60f / bpm;
        timer = 0;
        beatCounter = 0;
    }

    void Update()
    {
        // for testing if quarter lines spawn correctly on time; start and stop by pressing space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playing = !playing;
        }

        if (playing) PlayQuarterNotes();
        else
        {
            timer = 0; 
            beatCounter = 0;
        }
    }

    void PlayQuarterNotes()
    {
        // start spawning a line right away
        if (beatCounter == 0)
        {
            SpawnQuarterLine();
            // increase beatcounter
            // beat counter keeps track of which beat we're currently on
            beatCounter++;
        }
        else
        {
            // increase timer by amount of ms passed between frames
            timer += Time.deltaTime;

            // if it's time for the new quarternote line --> spawn new line
            // 60f/bpm is the duration of one beat in s
            // we multiply by the beatCounter to get the accurate time of the next beat at which to play the quarternote line
            if (timer >= 60f / bpm * beatCounter)
            {
                SpawnQuarterLine();
                beatCounter++;
            }
        }
    }


    void SpawnQuarterLine()
    {
        //intantiate a new quarternote line and set position to the far right of the piano roll:
        GameObject clone = Instantiate(beatObj, this.gameObject.transform);
        clone.transform.position = new Vector3(xPos, 0, 0);

        // tell the clone the current bpm, the length of the piano roll in beats, and the target value of position.x (how far it needs to travel to the left)
        clone.GetComponent<Notes>().NoteSetUp(bpm, beats, transform.position.x - bg.transform.localScale.x / 2f);
    }







    //----------- not yet up to date:


    void SpawnNote()
    {
        GameObject clone = Instantiate(noteObj, this.gameObject.transform);
        // missing: set correct y Pos according to line
        clone.transform.position = new Vector3(xPos, 0, 0);
        clone.GetComponent<Notes>().NoteSetUp(bpm, beats, transform.position.x - bg.transform.localScale.x / 2f);
    }

    // different way of doing it:
    // spawn one bar at a time
    IEnumerator SpawnQuarterTicks()
    {
        SpawnQuarterLine();
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(length4s);
            SpawnQuarterLine();
        }
    }
}
