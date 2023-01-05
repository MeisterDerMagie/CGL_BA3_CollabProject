using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.Netcode;

public enum RecordingState
{
    WAITTOSTART,
    COUNTIN,
    REC,
    IDLE
}

public struct RecordingNote
{
    public float timeStamp;
    public int soundID;
}

public class RecordInput : MonoBehaviour
{
    [SerializeField] private RecordingUI _recordingUI;

    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private AudioRoll _audioRoll;
    private BackingTrack _backingTrack;
    private BeatMapping _beatMapping;
    private PianoRollRecording _pianoRoll;
    private NoteSpawner _spawner;

    [SerializeField] private GameObject recFrame;

    public RecordingState recordingState;

    private List<RecordingNote> recordedTimeStamps; // technically obsolete since we map to grid right away, but we're still keeping track of the exact time stamps

    public List<Eighth> recordedBar;
    private List<Eighth> safetyCopyBar; // safety copy to go back to 

    private float timer;
    [HideInInspector] public bool stageEnded;

    bool spawnNote;

    // Start is called before the first frame update
    void Start()
    {
        spawnNote = false;

        if (NetworkManager.Singleton.IsServer) return;

        _audioRoll.SetUpAllInstances();

        _backingTrack = _audioRoll.gameObject.GetComponent<BackingTrack>();
        _backingTrack.beatUpdated += NextBeat;

        _beatMapping = GetComponent<BeatMapping>();
        _pianoRoll = _audioRoll.gameObject.GetComponentInParent<PianoRollRecording>();
        //_pianoRoll.PlayRecording(true, false);
        _recordingUI.playingBack = false;
        _recordingUI.SetPlayButton();

        _spawner = _pianoRoll.GetComponent<NoteSpawner>();

        recordingState = RecordingState.IDLE;

        recordedTimeStamps = new List<RecordingNote>();

        recordedBar = new List<Eighth>();
        safetyCopyBar = new List<Eighth>();

        WriteEmptyBar(safetyCopyBar);
        WriteEmptyBar(recordedBar);

        stageEnded = false;

        timer = 0;

        recFrame.SetActive(false);
        //countInText.gameObject.SetActive(false);

        // set sound IDs correctly

        /*
        if (!NetworkManager.Singleton.IsServer)
        {
            List<uint> instrumentIds = PlayerData.LocalPlayerData.InstrumentIds;

            List<Instrument> instruments = new List<Instrument>();
            foreach (uint instrumentId in instrumentIds)
            {
                Instrument instrument = InstrumentsManager.Instance.GetInstrument(instrumentId);
                instruments.Add(instrument);
            }

            //... do stuff with intsruments

            instruments[0].soundEvent...
        }
        */
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton.IsServer) return;
        _backingTrack.beatUpdated -= NextBeat;
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsServer) return;
        if (stageEnded) return;

        // increase timer if we are counting in or recording
        // we start the timer once we start counting in, in case the first input is a little before the beginning of the beat
        if (recordingState == RecordingState.COUNTIN || recordingState == RecordingState.REC) timer += Time.deltaTime;

        for (int i = 0; i < keyInputs.Length; i++)
        {
            if (Input.GetKeyDown(keyInputs[i]))
            {
                // play Sound
                _audioRoll.PlayerInputSound(i);

                // if recording / count in
                if (recordingState == RecordingState.REC || recordingState == RecordingState.COUNTIN)
                {
                    // save input to list of time stamps
                    RecordingNote n = new RecordingNote();
                    n.soundID = PlayerData.LocalPlayerData.InstrumentIds[i];
                    n.timeStamp = timer;
                    recordedTimeStamps.Add(n);

                    // process input and save to grid
                    _beatMapping.MapRecording(n);

                    // spawn note on piano roll
                    if (spawnNote) _spawner.SpawnNoteAtLocationMarker(i, _pianoRoll.bpm);
                }
            }
        }
    }

    public void RecordButton()
    {
        if (NetworkManager.Singleton.IsServer) return;

        // if we're in idle state, wait to start counting in at next available 1 and start recording
        if (recordingState == RecordingState.IDLE)
        {
            // waitToStartRecording at next 1 with a count in of one bar
            recordingState = RecordingState.WAITTOSTART;

            _pianoRoll.gameObject.GetComponent<NoteSpawner>().DeleteActiveNotes();

            // safety copy of bar:
            safetyCopyBar.Clear();
            foreach (Eighth e in recordedBar)
            {
                safetyCopyBar.Add(e);
            }
            WriteEmptyBar(recordedBar);

            _pianoRoll.ControlPlayback(PianoRollRecording.RecPBStage.ONLYLINES);

            _recordingUI.playingBack = false;
            //_recordingUI.SetPlayButton();

            // Update UI:
            recFrame.SetActive(true);
            _recordingUI.UpdateCountIn(true, "...");
        }
        else
        {
            // stop recording
            StopRecording();

            // if we're stopping the recording before we started to record anything --> return to safety copy
            if (recordingState != RecordingState.REC)
            {
                recordedBar.Clear();
                foreach (Eighth e in safetyCopyBar)
                {
                    recordedBar.Add(e);
                }
            }
        }
    }

    public void DeleteButton()
    {
        if (NetworkManager.Singleton.IsServer) return;

        if (recordingState != RecordingState.IDLE) return;

        recordedBar.Clear();
        WriteEmptyBar(recordedBar);

        _pianoRoll.ControlPlayback(PianoRollRecording.RecPBStage.INACTIVE);
    }

    void NextBeat()
    {
        if (NetworkManager.Singleton.IsServer) return;

        if (_backingTrack.lastBeat == 1)
        {
            switch (recordingState)
            {
                case RecordingState.IDLE:
                    // do nothing
                    break;
                case RecordingState.WAITTOSTART:
                    recordingState = RecordingState.COUNTIN;
                    _spawner.ActivateStartLine();
                    _spawner.isRecording = true;

                    // activate text field + set to one
                    _recordingUI.UpdateCountIn(true, "4");

                    // reset timer for recording:
                    timer = 0;

                    _beatMapping.PrepareRecording();

                    break;
                case RecordingState.COUNTIN:
                    recordingState = RecordingState.REC;
                    _spawner.isRecording = false;

                    _recordingUI.UpdateCountIn(true, "REC");
                    _recordingUI.playingBack = true;
                    _recordingUI.SetPlayButton();
                    break;
                case RecordingState.REC:
                    StopRecording();
                    recordedTimeStamps.Clear();
                    
                    break;
            }
        }
        else
        {
            if (recordingState == RecordingState.COUNTIN)
            {
                if (_backingTrack.lastBeat == 1)
                    _recordingUI.UpdateCountIn(true, "4");
                else if (_backingTrack.lastBeat == 3)
                    _recordingUI.UpdateCountIn(true, "3");
                else if (_backingTrack.lastBeat == 5)
                    _recordingUI.UpdateCountIn(true, "2");
                else if (_backingTrack.lastBeat == 7)
                {
                    _recordingUI.UpdateCountIn(true, "1");
                    spawnNote = true;
                }
            }
        }
    }

    public void StopRecording()
    {
        if (NetworkManager.Singleton.IsServer) return;

        recordingState = RecordingState.IDLE;

        // set UI object inactive
        recFrame.SetActive(false);
        _recordingUI.UpdateCountIn(false, "");
        //countInText.gameObject.SetActive(false);

        _spawner.DeactivateStartAndEndLine();

        _pianoRoll.ControlPlayback(PianoRollRecording.RecPBStage.INACTIVE);
        _recordingUI.playingBack = false;
        _recordingUI.SetPlayButton();
        _spawner.isRecording = false;

        spawnNote = false;
    }

    private void WriteEmptyBar(List<Eighth> _list)
    {
        if (NetworkManager.Singleton.IsServer) return;

        _list.Clear();

        for (int i = 0; i < 8; i++)
        {
            Eighth e = new Eighth();
            e.contains = false;
            e.instrumentID = -1;
            _list.Add(e);
        }
    }



#if UNITY_EDITOR
    private void OnGUI()
    {
        //GUILayout.Box($"recording State: {recordingState}");
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
