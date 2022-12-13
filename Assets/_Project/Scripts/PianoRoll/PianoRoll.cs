using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoRoll : MonoBehaviour
{
    [SerializeField] private float bpm = 110f;
    [Tooltip("Amount of lines in piano roll, so how many different kinds of notes spawned on top of each other")]
    [SerializeField] private int lines = 4;
    [Tooltip("Length of Piano Roll in beats, how many beats fit into piano roll")]
    [SerializeField] private int beats = 8;

    [SerializeField] private GameObject beatObj;
    [SerializeField] private GameObject noteObj;
    [SerializeField] private GameObject bg;

    float xPos; // where new notes should be spawned
    float length4s; // length of the quarter notes
    float timer;
    int beatCounter;
    bool playing;

    // Start is called before the first frame update
    void Start()
    {
        // get the right corner of the piano roll (where objects will be spawned)
        xPos = transform.position.x + bg.transform.localScale.x / 2f;
        length4s = bpm / 60f / 4f;
        timer = 0;
        beatCounter = 0;
    }

    void Update()
    {
        // for testing if quarter lines spawn correctly on time:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // way 1:
            //StartCoroutine(SpawnQuarterTicks());
            playing = !playing;
        }

        // way 2
        //return;
        if (playing) PlayQuarterNotes();
        else
        {
            timer = 0; 
            beatCounter = 0;
        }
    }

    void PlayQuarterNotes()
    {
        if (beatCounter == 0)
        {
            SpawnQuarterLine();
            beatCounter++;
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= bpm / 60f / 4f * beatCounter)
            {
                SpawnQuarterLine();
                beatCounter++;
            }
        }
    }

    IEnumerator SpawnQuarterTicks()
    {
        SpawnQuarterLine();
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(length4s);
            SpawnQuarterLine();
        }
    }

    void SpawnQuarterLine()
    {
        GameObject clone = Instantiate(beatObj, this.gameObject.transform);
        clone.transform.position = new Vector3(xPos, 0, 0);
        clone.GetComponent<Notes>().NoteSetUp(bpm, beats, transform.position.x - bg.transform.localScale.x / 2f);
        clone.SetActive(true);
    }

    void SpawnNote()
    {
        GameObject clone = Instantiate(noteObj, this.gameObject.transform);
        // missing: set correct y Pos according to line
        clone.transform.position = new Vector3(xPos, 0, 0);
        clone.GetComponent<Notes>().NoteSetUp(bpm, beats, transform.position.x - bg.transform.localScale.x / 2f);
    }
}
