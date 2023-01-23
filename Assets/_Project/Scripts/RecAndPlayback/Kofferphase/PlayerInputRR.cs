using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputRR : MonoBehaviour
{
    public bool active;
    private bool recording;

    public bool scorePlayability;

    private float startScoring;

    [SerializeField] private AudioRoll _audioRoll;
    private BeatMapping _beatMapping;
    private PianoRollTLKoffer _timeline;
    private BackingTrack _backingTrack;

    [Space]
    [SerializeField] private KeyCode[] keyInputs;
    [SerializeField] private float cooldown = 0.1f;

    int player;

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
                    //_audioRoll.PlayerInputSound(i);
                    _audioRoll.TestSound(i);
                    StartCoroutine(ButtonCoolDown());

                    if (recording)
                    {
                        RecordingNote n = new RecordingNote();

                        if (_timeline.testLocally) n.soundID = i;
                        else n.soundID = PlayerData.LocalPlayerData.InstrumentIds[i];

                        n.timeStamp = _backingTrack.timeSinceStart - startScoring;

                        _beatMapping.ScoreAccuracy(n, scorePlayability, player);
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

    public void StartRecording()
    {
        recording = true;
        startScoring = _backingTrack.timeSinceStart;
    }

    public void StopRecording()
    {
        recording = false;
    }

    public void AccuracyStart(float bpm, int player)
    {
        StartCoroutine(StartAccuracy(60f / bpm / 4f));
    }

    IEnumerator StartAccuracy(float duration)
    {
        yield return new WaitForSeconds(duration);
        scorePlayability = true;
    }
}
