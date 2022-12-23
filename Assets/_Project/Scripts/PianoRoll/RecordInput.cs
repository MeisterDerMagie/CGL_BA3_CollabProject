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

public struct RecordingNote
{
    public float timeStamp;
    public int soundID;
}

public class RecordInput : MonoBehaviour
{
    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private AudioRoll _audioRoll;
    private BackingTrack _backingTrack;
    private BeatMapping _beatMapping;

    public RecordingState recordingState;

    private List<RecordingNote> recordedTimeStamps;

    public List<Eighth> recordedBar;
    private List<Eighth> previousBar; // safety copy to go back to 

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        _audioRoll.SetUpAllInstances();

        _backingTrack = _audioRoll.gameObject.GetComponent<BackingTrack>();
        _backingTrack.beatUpdated += NextBeat;

        _beatMapping = GetComponent<BeatMapping>();

        recordingState = RecordingState.Idle;

        recordedTimeStamps = new List<RecordingNote>();

        recordedBar = new List<Eighth>();
        previousBar = new List<Eighth>();

        WriteEmptyBar(previousBar);
        WriteEmptyBar(recordedBar);

        timer = 0;
    }

    private void OnDestroy()
    {
        _backingTrack.beatUpdated -= NextBeat;
    }

    // Update is called once per frame
    void Update()
    {
        // increase timer if we are counting in or recording
        // we start the timer once we start counting in, in case the first input is a little before the beginning of the beat
        if (recordingState == RecordingState.CountIn || recordingState == RecordingState.Recording) timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            // waitToStartRecording at next 1 with a count in of one bar
            recordingState = RecordingState.WaitToStart;

            // MISSING: Stop Playback
        }

        for (int i = 0; i < keyInputs.Length; i++)
        {
            if (Input.GetKeyDown(keyInputs[i]))
            {
                // play Sound
                _audioRoll.PlayerInputSound(i);

                // if recording / count in
                if (recordingState == RecordingState.Recording || recordingState == RecordingState.CountIn)
                {
                    // save input to list of time stamps
                    RecordingNote n = new RecordingNote();
                    n.soundID = i;
                    n.timeStamp = timer;
                    recordedTimeStamps.Add(n);

                    // process input and save to grid
                    _beatMapping.MapRecording(n);
                }
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
                    // activate text field + set to one

                    // reset timer for recording:
                    timer = 0;

                    // safety copy of bar:
                    previousBar = recordedBar;
                    WriteEmptyBar(recordedBar);

                    _beatMapping.PrepareRecording();

                    break;
                case RecordingState.CountIn:
                    recordingState = RecordingState.Recording;

                    // deactivate text field

                    break;
                case RecordingState.Recording:
                    StopRecording();
                    recordedTimeStamps.Clear();
                    // deal with inputs
                    // set count in object inactive
                    break;
            }
        }
        else
        {
            if (recordingState == RecordingState.CountIn)
            {
                // set text field
            }
        }
    }

    public void StopRecording()
    {
        recordingState = RecordingState.Idle;
    }

    private void WriteEmptyBar(List<Eighth> _list)
    {
        _list.Clear();

        for (int i = 0; i < 8; i++)
        {
            Eighth e = new Eighth();
            e.contains = false;
            e.soundID = -1;
            _list.Add(e);
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.Box($"recording State: {recordingState}");
    }
#endif


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
