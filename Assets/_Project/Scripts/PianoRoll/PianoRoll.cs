using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoRoll : MonoBehaviour
{
    [SerializeField] private int bpm = 110;
    [Tooltip("Amount of lines in piano roll, so how many different kinds of notes spawned on top of each other")]
    [SerializeField] private int lines = 4;
    [Tooltip("Length of Piano Roll in beats, how many beats fit into piano roll")]
    [SerializeField] private int beats = 8;

    [SerializeField] private GameObject beatObj;
    [SerializeField] private GameObject noteObj;
    [SerializeField] private GameObject bg;

    float timer;
    float xPos;

    // Start is called before the first frame update
    void Start()
    {
        // get the right corner of the piano roll (where objects will be spawned)
        xPos = transform.position.x + bg.transform.localScale.x / 2;
    }

    void Update()
    {
        // for testing the spawn
        if (Input.GetKey(KeyCode.Space))
        {
            timer += Time.deltaTime;

            // spawn Viertel:
            if (timer >= bpm/60)
            {
                GameObject clone = Instantiate(beatObj, this.gameObject.transform);
                clone.transform.position = new Vector3(xPos, 0, 0);
                clone.GetComponent<Notes>().NoteSetUp(bpm, beats, bg.transform.localScale.x);
            }
        }
        else timer = 0;
    }
}
