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
    [HideInInspector] public RecordingUI _recordingUI;

    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private AudioRoll _audioRoll;
    private BackingTrack _backingTrack;
    private BeatMapping _beatMapping;
    private PianoRoll _pianoRoll;

    [SerializeField] private GameObject recFrame;
    //[SerializeField] private TMPro.TextMeshProUGUI countInText;

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
        _pianoRoll = _audioRoll.gameObject.GetComponentInParent<PianoRoll>();
        _pianoRoll.PlayRecording(true, false);
        _recordingUI._audio = false;

        recordingState = RecordingState.Idle;

        recordedTimeStamps = new List<RecordingNote>();

        recordedBar = new List<Eighth>();
        previousBar = new List<Eighth>();

        WriteEmptyBar(previousBar);
        WriteEmptyBar(recordedBar);

        timer = 0;

        recFrame.SetActive(false);
        //countInText.gameObject.SetActive(false);
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

    public void RecordButton()
    {
        if (recordingState == RecordingState.Idle)
        {
            // waitToStartRecording at next 1 with a count in of one bar
            recordingState = RecordingState.WaitToStart;

            _pianoRoll.gameObject.GetComponent<NoteSpawner>().DeleteActiveNotes();

            // safety copy of bar:
            previousBar = recordedBar;
            WriteEmptyBar(recordedBar);
            _pianoRoll.PlayRecording(true, false);
            _recordingUI._audio = false;

            // Update UI:
            recFrame.SetActive(true);
            _recordingUI.UpdateCountIn(true, "...");
            /*
            countInText.gameObject.SetActive(true);
            countInText.text = "...";
            */
        }
        else
        {
            // stop recording
            StopRecording();
            recordedBar = previousBar;
            _pianoRoll.PlayRecording(true, false);
            _recordingUI._audio = false;
        }
    }

    public void DeleteButton()
    {
        recordedBar.Clear();
        WriteEmptyBar(recordedBar);
    }

    public void PlayButton()
    {
        // turn on audio
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

                    // activate text field + set to one
                    _recordingUI.UpdateCountIn(true, "1");
                    /*
                    countInText.gameObject.SetActive(true);
                    countInText.text = "1";
                    */

                    // reset timer for recording:
                    timer = 0;

                    _beatMapping.PrepareRecording();

                    break;
                case RecordingState.CountIn:
                    recordingState = RecordingState.Recording;

                    _recordingUI.UpdateCountIn(true, "REC");
                    //countInText.text = "REC";
                    _pianoRoll.PlayRecording(true, true);
                    _recordingUI._audio = true;
                    break;
                case RecordingState.Recording:
                    StopRecording();
                    recordedTimeStamps.Clear();
                    
                    break;
            }
        }
        else
        {
            if (recordingState == RecordingState.CountIn)
            {
                if (_backingTrack.lastBeat == 1)
                {
                    _recordingUI.UpdateCountIn(true, "1");
                    //countInText.text = "1";
                }
                else if (_backingTrack.lastBeat == 3)
                {
                    _recordingUI.UpdateCountIn(true, "2");
                    //countInText.text = "2";
                }
                else if (_backingTrack.lastBeat == 5)
                {
                    _recordingUI.UpdateCountIn(true, "3");
                    //countInText.text = "3";
                }
                else if (_backingTrack.lastBeat == 7)
                {
                    _recordingUI.UpdateCountIn(true, "AND");
                    //countInText.text = "AND";
                }
            }
        }
    }

    public void StopRecording()
    {
        recordingState = RecordingState.Idle;

        // set UI object inactive
        recFrame.SetActive(false);
        _recordingUI.UpdateCountIn(false, "");
        //countInText.gameObject.SetActive(false);
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
    // then count in at next timeline Beat 1 --> blink red + UI 1 - 2 - 3 - AND
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
