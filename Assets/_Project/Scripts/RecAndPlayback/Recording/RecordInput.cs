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
    [SerializeField] private CountInSounds _countInSounds;

    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private AudioRoll _audioRoll;
    private BackingTrack _backingTrack;
    private BeatMapping _beatMapping;
    private PianoRollRecording _pianoRoll;
    private NoteSpawner _spawner;

    [SerializeField] private GameObject recFrame;

    public RecordingState recordingState;

    private List<RecordingNote> recordedTimeStamps; // technically obsolete since we map to grid right away, but we're still saving track of the exact time stamps

    public List<List<Eighth>> recording;
    public List<List<Eighth>> copy; // safety copy to get back to
    private int barTimer;

    private float timer;
    [HideInInspector] public bool stageEnded;

    bool spawnNote;
    bool active;
    [SerializeField] float buttonCooldown = 0.1f;

    void Start()
    {
        spawnNote = false;
        active = true;

        if (NetworkManager.Singleton.IsServer) return;

        _audioRoll.SetUpAllInstances();
        _backingTrack = _audioRoll.gameObject.GetComponent<BackingTrack>();
        _backingTrack.beatUpdated += NextBeat;
        _beatMapping = GetComponent<BeatMapping>();
        _pianoRoll = _audioRoll.gameObject.GetComponentInParent<PianoRollRecording>();
        _beatMapping.SetUp(_pianoRoll.bpm);
        _recordingUI.playingBack = false;
        _recordingUI.SetPlayButton();

        _spawner = _pianoRoll.GetComponent<NoteSpawner>();

        recordingState = RecordingState.IDLE;

        recordedTimeStamps = new List<RecordingNote>();

        stageEnded = false;
        timer = 0;
        barTimer = 0;

        recFrame.SetActive(false);
        _beatMapping.PrepareRecording();

        recording = new List<List<Eighth>>();
        WriteEmptyBar(recording);
        copy = new List<List<Eighth>>();
        WriteEmptyBar(copy);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton.IsServer) return;
        _backingTrack.beatUpdated -= NextBeat;
    }

    void Update()
    {
        if (NetworkManager.Singleton.IsServer) return;
        if (stageEnded) return;

        // increase timer if we are counting in or recording
        // we start the timer once we start counting in, in case the first input is a little before the beginning of the beat
        if (recordingState == RecordingState.COUNTIN || recordingState == RecordingState.REC) timer += Time.deltaTime;

        if (active)
        {
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
                    StartCoroutine(ButtonCooldown());
                }
            }
        }
    }

    IEnumerator ButtonCooldown()
    {
        active = false;
        yield return new WaitForSeconds(buttonCooldown);
        active = true;
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
            copy.Clear();
            for (int b = 0; b < Constants.RECORDING_LENGTH; b++)
            {
                List<Eighth> _bar = new List<Eighth>();
                foreach (Eighth e in recording[b])
                {
                    _bar.Add(e);
                }
                copy.Add(_bar);
            }
            
            WriteEmptyBar(recording);

            _pianoRoll.ControlPlayback(PianoRollRecording.RecPBStage.ONLYLINES);

            _recordingUI.playingBack = false;
            _recordingUI.SetPlayButton();
            _recordingUI.playButtonActive = false;

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
                recording.Clear();
                for (int b = 0; b < Constants.RECORDING_LENGTH; b++)
                {
                    List<Eighth> bar = new List<Eighth>();

                    foreach (Eighth e in copy[b])
                    {
                        bar.Add(e);
                    }

                    recording.Add(bar);
                }
            }

            _recordingUI.playingBack = false;
            _recordingUI.SetPlayButton();

            _spawner.DeactivateStartAndEndLine();
            _spawner.isRecording = false;
        }
    }

    public void DeleteButton()
    {
        if (NetworkManager.Singleton.IsServer) return;

        if (recordingState != RecordingState.IDLE) return;

        recording.Clear();
        WriteEmptyBar(recording);

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
                    _countInSounds.PlayCountIn(4);

                    // reset timer for recording:
                    timer = 0;

                    //_beatMapping.PrepareRecording();

                    break;
                case RecordingState.COUNTIN:
                    recordingState = RecordingState.REC;
                    _recordingUI.UpdateCountIn(true, "REC");
                    _recordingUI.playingBack = true;
                    _recordingUI.SetPlayButton();

                    barTimer = 0;
                    break;
                case RecordingState.REC:
                    barTimer++;

                    if (barTimer >= Constants.RECORDING_LENGTH)
                    {
                        StopRecording();
                        recordedTimeStamps.Clear();
                    }
                    break;
            }
        }
        else
        {
            if (recordingState == RecordingState.COUNTIN)
            {
                if (_backingTrack.lastBeat == 1)
                {
                    _recordingUI.UpdateCountIn(true, "4");
                    _countInSounds.PlayCountIn(4);
                }
                else if (_backingTrack.lastBeat == 3)
                {
                    _recordingUI.UpdateCountIn(true, "3");
                    _countInSounds.PlayCountIn(3);
                }
                else if (_backingTrack.lastBeat == 5)
                {
                    _recordingUI.UpdateCountIn(true, "2");
                    _countInSounds.PlayCountIn(2);
                }
                else if (_backingTrack.lastBeat == 7)
                {
                    _recordingUI.UpdateCountIn(true, "1");
                    _countInSounds.PlayCountIn(1);
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

        _pianoRoll.ControlPlayback(PianoRollRecording.RecPBStage.INACTIVE);
        _recordingUI.playingBack = false;
        _recordingUI.SetPlayButton();
        _recordingUI.playButtonActive = true;

        spawnNote = false;
    }

    private void WriteEmptyBar(List<List<Eighth>> _list)
    {
        if (NetworkManager.Singleton.IsServer) return;

        _list.Clear();

        for (int b = 0; b < Constants.RECORDING_LENGTH; b++)
        {
            List<Eighth> bar = new List<Eighth>();
            for (int i = 0; i < 8; i++)
            {
                Eighth e = new Eighth();
                e.contains = false;
                e.instrumentID = -1;
                bar.Add(e);
            }
            _list.Add(bar);
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
