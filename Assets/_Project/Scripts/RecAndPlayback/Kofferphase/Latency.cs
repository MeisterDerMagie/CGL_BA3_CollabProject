using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latency : MonoBehaviour
{
    CSWriter _csWriter;
    BackingTrack _backingTrack;
    AudioRoll _audio;

    float bpm = 110f;
    [SerializeField] private int amountOfTestBars = 50;
    int barCounter;
    float time;

    [SerializeField] private List<float> targetTimeStamps;
    float dispersion;

    bool active;
    int lastCorrectStamp;

    void Start()
    {
        _csWriter = GetComponent<CSWriter>();

        _backingTrack = GetComponentInChildren<BackingTrack>();
        _backingTrack.beatUpdated += NextBeat;

        _audio = GetComponentInChildren<AudioRoll>();
        _audio.TestSetup();

        CreateCompareList();

        // start music
        _backingTrack.StartMusic();
        time = 0;
        barCounter = 0;
        active = true;
        lastCorrectStamp = 0;
    }

    private void OnDestroy()
    {
        _backingTrack.beatUpdated -= NextBeat;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            time += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // play audio
                _audio.TestSound(0);

                // compare note and save to list
                SaveNote(time);
            }
        }
    }

    void NextBeat()
    {
        if (_backingTrack.lastBeat == 1)
            barCounter++;

        if (_backingTrack.lastBeat == 1 && barCounter == 1)
            time = 0;

        if (barCounter == amountOfTestBars + 3)
        {
            active = false;
            _csWriter.WriteFile();
            _backingTrack.StopMusic();
            Debug.Log("Done Testing");
        }
    }

    void CreateCompareList()
    {
        float introTime = 60f / bpm * 4f;
        dispersion = 60f / bpm / 2f;

        targetTimeStamps = new List<float>();

        // for every bar we want to test
        for (int b = 0; b < amountOfTestBars; b++)
        {
            // save the time stamp of each quarter note in that bar
            for (int q = 0; q < 4; q++)
            {
                // time stamp is intro time + durationn of a bar * which bar we are in + duration of a quarter note * which quarter note we are in
                float timeStamp = introTime + (60f / bpm) * 4 * b + (60f / bpm) * q;
                targetTimeStamps.Add(timeStamp);
            }
        }
    }

    void SaveNote(float timeStamp)
    {
        // which eighth does it belong to
        for (int i = lastCorrectStamp; i < targetTimeStamps.Count; i++)
        {
            if (timeStamp >= targetTimeStamps[i] - dispersion && timeStamp <= targetTimeStamps[i] + dispersion)
            {
                // send over to writer script list
                _csWriter.AddNewNote(targetTimeStamps[i], timeStamp);

                // reset last correct Stamp to this (so we don't go through entire list from beginning
                lastCorrectStamp = i;

                return;
            }
        }
    }
}
