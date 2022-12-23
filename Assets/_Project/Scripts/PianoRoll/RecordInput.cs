using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class RecordInput : MonoBehaviour
{
    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private AudioRoll _audioRoll;
    [SerializeField] private EventReference snare;
    private FMOD.Studio.EventInstance instance;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        //instance = RuntimeManager.CreateInstance(snare);
        SetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            // start recording
        }

        /*
        if (Input.GetKeyDown(keyInputs[0])) //_audioRoll.PlaySound(0);
        {
            instance.setPaused(false);
            instance.release();
            SetInstance();
        }
        */

        for (int i = 0; i < keyInputs.Length; i++)
        {
            if (Input.GetKeyDown(keyInputs[i]))
            {
                // play Sound
                //_audioRoll.PlaySound(i);
                instance.setPaused(false);
                instance.release();
                SetInstance();

                // if recording:
                // if pressed --> save input + process input
            }
        }
    }

    void SetInstance()
    {
        instance = RuntimeManager.CreateInstance(snare);
        instance.start();
        instance.setPaused(true);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        //GUILayout.Box($"time.deltaTime = {timer}");
    }
#endif


    // have keys input as variables for int soundID
    // --> fixed sound ID 1-4 (or 1-8)
    // --> array of KeyCodes maybe?

    // struct for player input of each keyInput pressed that corresponded to a soundID
    // float timestamp
    // int soundID

    // save struct in a List<input> ?
    // or process directly and map to nearest beat

    // press r to record
    // then count in at next timeline Beat 1 --> blink red + GUI 1 - 2 - 3 - AND
    // spawn note immediately on location marker line and move to the end
    // button cooldown
    // start timer with countdown and the subtract one bar from timer for every note when grid mapping
    // prolly I will have to grid map by hand OR
    // for every input go through every eighth
    // have variable for variance (which is +- the timestamp of an eighth (+ 1 bar because of count in) and half the duration of an eighth)
    // if it is near that eighth --> save in bar 




    // for checking the correct playback --> scoring
    // same principle as grid mapping?
    // for every note played by player
    // save player input bar by bar; with new bar, save to a new List
    // if the last note in the list is near, close or bang on the first beat in the next bar --> remove from this list and add to list of next bar
    // go through every eighth of the bar
    // have variables (multiple) of variance +- the timestamp of the eighth (+ x bars since beginning of playback; or reset timer with every new bar and list)
    // check if it's near, close, bang on --> check if it's also the right soundID used --> give points
    // if the note did not come near any eighth --> subtract points
}
