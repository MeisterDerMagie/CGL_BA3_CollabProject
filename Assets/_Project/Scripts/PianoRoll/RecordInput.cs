using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public enum RecordingState
{
    WaitToStart,
    CountIn,
    Recording,
    Idle
}

public class RecordInput : MonoBehaviour
{
    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private AudioRoll _audioRoll;
    private BackingTrack _backingTrack;

    public RecordingState recordingState;

    // Start is called before the first frame update
    void Start()
    {
        _audioRoll.SetUpAllInstances();

        _backingTrack = _audioRoll.gameObject.GetComponent<BackingTrack>();
        _backingTrack.beatUpdated += NextBeat;

        recordingState = RecordingState.Idle;
    }

    private void OnDestroy()
    {
        _backingTrack.beatUpdated -= NextBeat;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // waitToStartRecording at next 1 with a count in of one bar
            recordingState = RecordingState.WaitToStart;
        }

        for (int i = 0; i < keyInputs.Length; i++)
        {
            if (Input.GetKeyDown(keyInputs[i]))
            {
                // play Sound
                _audioRoll.PlayerInputSound(i);

                // if recording:
                // if pressed --> save input + process input
            }
        }
    }

    void NextBeat()
    {
        if (_backingTrack.lastBeat == 1)
        {
            switch (recordingState)
            {
                case RecordingState.Idle:
                    // do nothing
                    break;
                case RecordingState.WaitToStart:
                    recordingState = RecordingState.CountIn;
                    // set count in object active
                    // start the timer;
                    break;
                case RecordingState.CountIn:
                    recordingState = RecordingState.Recording;
                    // set recording stuff
                    break;
                case RecordingState.Recording:
                    recordingState = RecordingState.Idle;
                    // deal with inputs
                    // set count in object inactive
                    break;
            }
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.Box($"recording State: {recordingState}");
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
