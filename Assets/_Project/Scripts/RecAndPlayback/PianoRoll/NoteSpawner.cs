using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script sits on the Piano Roll. The Piano roll keeps track of bars / time and tells the Spawner when to spawn which objects.
/// The Spawner spawns notes, lines, and activates/deactivates idle lines
/// </summary>

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private AudioRoll _audioRoll;

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
    /*
    [Tooltip("how many beats are to the right side of the location marker; how many beats can you preview before they need to be played")]
    [SerializeField] private int previewLength = 6;
    */

    float xPos; // where new notes should be spawned
    [SerializeField]List<float> yPos; // height of spawned notes which depends on the lineHeight of piano roll
    int beats; // how often are lines spawned, every beat or if 2 every other beat

    public List<float> posLines; // where all lines should be spawned idle, where the downbeats are on piano roll
    private List<Transform> idleLines; // keep track of all idle lines
    private List<GameObject> currentNotes; // keep track of currently active notes to deactivate when hitting pause
    private List<GameObject> currentLines;

    // Start is called before the first frame update
    void Start()
    {
        // get the right corner of the piano roll (where objects will be spawned)
        xPos = transform.localPosition.x + bg.transform.localScale.x / 2f;

        currentNotes = new List<GameObject>();
        currentLines = new List<GameObject>();

        #region Calculate Positions for all idle and start lines
        posLines = new List<float>();
        beats = 1;

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
            if (i == 1 || i == 3 || i == 5 || i  == 7 ||  i == 9 || i == 11) idleLines[i].GetComponent<Notes>().MakeOpaque();
        }
        #endregion

        #region Calculate Height for all notes to be spawned
        float yTop = transform.localPosition.y + bg.transform.localScale.y / 2;
        // when between lines use lineHeight instead of lH - 1
        float distance = bg.transform.localScale.y / (lineHeight - 1);

        yPos = new List<float>();

        // add highest line:
        // when used between lines not on them use: yPos.Add(yTop - distance / 2);
        yPos.Add(yTop);
        for (int  i = 1;  i < lineHeight;  i++)
        {
            float value = yPos[i - 1] - distance;
            if (i == lineHeight - 1) value = -yTop;
            yPos.Add(value);
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
                Destroy(obj);
            currentNotes.Clear();
            foreach (GameObject obj in currentLines)
                Destroy(obj);
            currentLines.Clear();
        }
        else
        {
            // if idle lines are deactivated, spawn new lines that move:
            for (int i = 0; i < posLines.Count; i++)
            {
                // instantiate a new line and set position to what is saved in posLines
                GameObject clone = Instantiate(beatObj, spawns.transform);
                clone.transform.localPosition = new Vector3(posLines[i], 0, 0);

                int a = -1;
                // tell the clone the current bpm, length of piano roll in beats, and target value of poision x (how far it needs to travel to the left)
                if (i == 1 || i == 3 || i == 5 || i == 7 || i == 9 || i == 11 || i == 13) a = 2;
                clone.GetComponent<Notes>().NoteSetUp(bpm, (i + 1) * beats, transform.localPosition.x - bg.transform.localScale.x / 2f, this,  -1, a);

                currentLines.Add(clone);
            }
        }
    }

    public void DeleteActiveNotes()
    {
        foreach (GameObject obj in currentNotes)
            Destroy(obj);
        currentNotes.Clear();
    }

    public void SpawnLines(float bpm, int number)
    {
        //intantiate a new quarternote line and set position to the far right of the piano roll:
        GameObject clone = Instantiate(beatObj, spawns.transform);
        clone.transform.localPosition = new Vector3(xPos, 0, 0);

        // tell the clone the current bpm, the length of the piano roll in beats, and the target value of position.x (how far it needs to travel to the left)
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.localPosition.x - bg.transform.localScale.x / 2f, this, -1, number);

        currentLines.Add(clone);
    }

    public void SpawnNote(int line, float bpm)
    {
        // instantiate a new note and set position to the far right of the piano roll
        GameObject clone = Instantiate(noteObj, spawns.transform);
        // y Pos is dependent on which note it is; calculation of the List happens in the start function of this script
        clone.transform.localPosition = new Vector3(xPos, yPos[line], 0);

        // tell the clone the current bpm, length of roll in beats, target position x
        clone.GetComponent<Notes>().NoteSetUp(bpm, beatLength, transform.localPosition.x - bg.transform.localScale.x / 2f, this, line);

        currentNotes.Add(clone);
    }

    public void RemoveNote(GameObject obj)
    {
        currentNotes.Remove(obj);
    }

}
