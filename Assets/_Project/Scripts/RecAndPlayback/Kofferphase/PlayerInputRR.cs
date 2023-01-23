using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputRR : MonoBehaviour
{
    public bool active;
    private bool recording;
    private float timer;
    public bool scorePlayability;

    [SerializeField] private AudioRoll _audioRoll;
    private BeatMapping _beatMapping;

    [Space]
    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private float cooldown = 0.1f;

    bool testLocally;

    float newTimer;
    List<RecordingNote> inputs;
    float lastEight;
    float halfEighthDuration;

    void Start()
    {
        _beatMapping = GetComponent<BeatMapping>();
        testLocally = GetComponentInParent<PianoRollTLKoffer>().testLocally;

        newTimer = 0;
        lastEight = 0;
        inputs = new List<RecordingNote>();

        halfEighthDuration = 60f / GetComponentInParent<PianoRollTLKoffer>().bpm / 4;
    }

    void Update()
    {
        timer += Time.deltaTime;
        newTimer += Time.deltaTime;

        if (active)
        {
            for (int i = 0; i < keyInputs.Length; i++)
            {
                if (Input.GetKeyDown(keyInputs[i]))
                {
                    // play Sound
                    //_audioRoll.PlayerInputSound(i);
                    _audioRoll.TestSound(i);
                    StartCoroutine(ButtonCoolDown());

                    if (recording)
                    {
                        RecordingNote n = new RecordingNote();

                        if (testLocally) n.soundID = i;
                        else n.soundID = PlayerData.LocalPlayerData.InstrumentIds[i];

                        n.timeStamp = timer;

                        _beatMapping.ScoreAccuracy(n, scorePlayability);
                    }
                }
            }
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

        if (!script.testLocally)
        {
            foreach (PlayerData player in script.sortedPlayers)
            {
                for (int i = 0; i < player.Recording.Count; i++)
                {
                    Eighth e = new Eighth();

                    e.contains = player.Recording[i].contains;
                    e.instrumentID = player.Recording[i].instrumentID;

                    allRecordings.Add(e);
                }
            }
        }
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
        }

        // tell beatmapping script to write compare to bars
        if (_beatMapping == null) _beatMapping = GetComponent<BeatMapping>();
        if (script.testLocally)
            _beatMapping.PrepareAccuracyScoring(bpm, allRecordings, script.testPlayerAmount);
        else _beatMapping.PrepareAccuracyScoring(bpm, allRecordings, script.sortedPlayers.Count);
    }

    public void NextBeat()
    {
        lastEight = newTimer;

        if (inputs.Count != 0)
            for (int i = 0; i < inputs.Count; i++)
            {

            }
    }

    public void StartRecording()
    {
        recording = true;
        timer = 0;
    }

    public void StopRecording()
    {
        recording = false;
    }

    public void AccuracyStart(float bpm)
    {
        StartCoroutine(StartAccuracy(60f / bpm / 4f));
    }

    IEnumerator StartAccuracy(float duration)
    {
        yield return new WaitForSeconds(duration);
        scorePlayability = true;
    }
}
