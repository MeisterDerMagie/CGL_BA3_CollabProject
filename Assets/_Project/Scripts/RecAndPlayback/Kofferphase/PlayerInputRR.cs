using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputRR : MonoBehaviour
{
    public bool active;
    private bool recording;

    public bool scorePlayability;

    private float startScoringDate;
    private float startScoringUnity;

    [SerializeField] private AudioRoll _audioRoll;
    private BeatMapping _beatMapping;
    private PianoRollTLKoffer _timeline;
    private BackingTrack _backingTrack;

    [Space]
    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private float cooldown = 0.1f;

    public int player;

    void Start()
    {
        _timeline = GetComponentInParent<PianoRollTLKoffer>();
        _beatMapping = GetComponent<BeatMapping>();
        _backingTrack = _audioRoll.GetComponent<BackingTrack>();

    }

    void Update()
    {
        if (active)
        {
            for (int i = 0; i < keyInputs.Length; i++)
            {
                if (Input.GetKeyDown(keyInputs[i]))
                {
                    // play Sound
                    if (_timeline.testLocally) _audioRoll.TestSound(i);
                    else _audioRoll.PlayerInputSound(i);

                    StartCoroutine(ButtonCoolDown());

                    if (recording)
                    {
                        RecordingNote n = new RecordingNote();

                        if (_timeline.testLocally) n.soundID = i;
                        else n.soundID = PlayerData.LocalPlayerData.InstrumentIds[i];

                        n.timeStamp = _backingTrack.timeSinceStart - startScoringDate;

                        _beatMapping.ScoreAccuracy(n, scorePlayability, player);
                    }
                }
            }
            /*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _audioRoll.TestSound(0);
                StartCoroutine(ButtonCoolDown());
                if(recording) _beatMapping.LatencyTestScoring(_backingTrack.timeSinceStart - startScoringDate, _backingTrack.timeSinceStartUnity - startScoringUnity);
            }
            */
        }
    }

    IEnumerator ButtonCoolDown()
    {
        active = false;
        yield return new WaitForSeconds(cooldown);
        active = true;
    }

    public void SetUpRecording(float bpm, PianoRollTLKoffer script)
    {
        // convert player data info into a list of eighth for beat mapping:
        List<Eighth> allRecordings = new List<Eighth>();
        // tell beatmapping script to write compare to bars
        if (_beatMapping == null) _beatMapping = GetComponent<BeatMapping>();

        if (!script.testLocally)
            _beatMapping.PrepareAccuracyScoringNetwork(bpm, script.sortedPlayers, _timeline.countInToRhythmRepeat);
        else
        {
            foreach (List<Eighth> list in script.testPlayers)
            {
                for (int i = 0; i < Constants.RECORDING_LENGTH * 8; i++)
                {
                    Eighth e = new Eighth();

                    e.contains = list[i].contains;
                    e.instrumentID = list[i].instrumentID;

                    allRecordings.Add(e);
                }
            }
            _beatMapping.PrepareAccuracyScoring(bpm, allRecordings, script.testPlayerAmount, _timeline.countInToRhythmRepeat);
        }
    }

    public void StartRecording()
    {
        recording = true;
        startScoringDate = _backingTrack.timeSinceStart;
        startScoringUnity = _backingTrack.timeSinceStartUnity;
    }

    public void StopRecording()
    {
        recording = false;
    }

    public void AccuracyStart(float bpm, int _player)
    {
        StartCoroutine(StartAccuracy(60f / bpm / 4f));
        player = _player;
    }

    IEnumerator StartAccuracy(float duration)
    {
        yield return new WaitForSeconds(duration);
        scorePlayability = true;
    }
}
