using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script sits on the Piano Roll. The Piano roll keeps track of bars / time and tells the Spawner when to spawn which objects.
/// </summary>

public class NoteSpawner : MonoBehaviour
{
    PianoRoll pianoRoll;

    [Header("Game Objects for spawning lines and notes")]
    [SerializeField] private GameObject beatObj; // the quarternote lines marking every beat
    [SerializeField] private GameObject noteObj; // the note spawned
    [SerializeField] private GameObject bg; // the background image of the piano roll, to get correct size
    [SerializeField] private GameObject idleL; // gameObject that holds spawned idle lines
    [SerializeField] private GameObject spawns; // gameObject that holds all spawns

    [Space]
    [Header("Attributes of Piano Roll")]
    [Tooltip("Amount of lines in piano roll, so how many different kinds of notes spawned on top of each other")]
    [SerializeField] private int lineHeight = 4;
    [Tooltip("Length of Piano Roll in beats, how many beats fit into piano roll")]
    [SerializeField] private int beatLength = 8;
    [Tooltip("how many beats are to the right side of the location marker; how many beats can you preview before they need to be played")]
    [SerializeField] private int previewLength = 6;

    float xPos; // where new notes should be spawned
    int beats; // how often are lines spawned, every beat or if 2 every other beat

    List<float> posLines; // where all lines should be spawned idle, where the downbeats are on piano roll
    //idle lines when piano roll is not playing
    private List<Transform> idleLines;
    private List<GameObject> currentNotes;

    // Start is called before the first frame update
    void Start()
    {
        pianoRoll = GetComponent<PianoRoll>();

        // get the right corner of the piano roll (where objects will be spawned)
        xPos = transform.localPosition.x + bg.transform.localScale.x / 2f;

        currentNotes = new List<GameObject>();

        #region Calculate Positions for all idle and start lines
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
            Transform clone = Instantiate(beatObj, idleL.transform).transform;
            idleLines.Add(clone.transform);
        }

        // set up first idle line at left hand corner of piano roll
        idleLines[0].localPosition = new Vector3(-xPos, 0, 0);

        // set up all other lines
        for (int i = 1; i < idleLines.Count; i++)
        {
            idleLines[i].gameObject.SetActive(true);
            idleLines[i].localPosition = new Vector3(posLines[i - 1], 0, 0);
        }
        #endregion
    }

    public void ActivateIdleLines(bool var, float bpm)
    {
        // de/activate idleLines
        idleL.SetActive(var);

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
                GameObject clone = Instantiate(beatObj, spawns.transform);
                clone.transform.localPosition = new Vector3(posLines[i], 0, 0);

                // tell the clone the current bpm, length of piano roll in beats, and target value of poision x (how far it needs to travel to the left)
                clone.GetComponent<Notes>().NoteSetUp(bpm, (i + 1) * beats, transform.localPosition.x - bg.transform.localScale.x / 2f, this);

                currentNotes.Add(clone);
            }
        }
    }

    public void SpawnLines(float bpm)
    {
        //intantiate a new quarternote line and set position to the far right of the piano roll:
        GameObject clone = Instantiate(beatObj, spawns.transform);
        clone.transform.localPosition = new Vector3(xPos, 0, 0);

        // tell the clone the current bpm, the length of the piano roll in beats, and the target value of position.x (how far it needs to travel to the left)
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.localPosition.x - bg.transform.localScale.x / 2f, this);

        currentNotes.Add(clone);
    }

    public void SpawnNote(int sample, float bpm)
    {
        GameObject clone = Instantiate(noteObj, spawns.transform);
        // missing: set correct y Pos according to line
        clone.transform.localPosition = new Vector3(xPos, 0, 0);
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.localPosition.x - bg.transform.localScale.x / 2f, this);

        currentNotes.Add(clone);
    }

    public void RemoveNote(GameObject obj)
    {
        currentNotes.Remove(obj);
    }
}
