using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoRollPrevKoffer : MonoBehaviour
{
    private PianoRollTimer _timer;
    private NoteSpawner _spawner;
    private PianoRollTLKoffer _timeline;

    int amountPlaybackPlayers;
    int currentPlayer;
    int currentBar;
    int barTimer;

    PianoRollTLKoffer.KofferStages currentStage;


    void Start()
    {
        // get references to all relevant scripts:
        _timer = GetComponent<PianoRollTimer>();
        _timeline = GetComponent<PianoRollTLKoffer>();
        _spawner = GetComponent<NoteSpawner>();

        currentPlayer = 0;
        currentBar = 0;

        currentStage = PianoRollTLKoffer.KofferStages.IDLE;
    }

    public void StartPlayback()
    {
        // deactivate idle lines and spawn lines across the piano roll (timeline script starts music)
        _spawner.ActivateIdleLines(false);
        _spawner.SpawnLinesOnRoll(_timeline.bpm);
        _spawner.spawnActive = true;

        // reset variables
        barTimer = _timer.previewBar;
        currentPlayer = 0;
        currentBar = 0;
        amountPlaybackPlayers = 1;

        // set current stage
        currentStage = PianoRollTLKoffer.KofferStages.COUNTINPB;
    }

    public void NextBeat()
    {
        if (currentStage == PianoRollTLKoffer.KofferStages.IDLE) return;

        PlayQuarterNote();

        // if we're playing back or repeating rhythm --> spawn notes
        if (currentStage == PianoRollTLKoffer.KofferStages.RHYTHMREPEAT || currentStage == PianoRollTLKoffer.KofferStages.PLAYBACK)
        {
            // with test players:

        }
    }

    void UpdateStage()
    {
        switch (currentStage)
        {
            case PianoRollTLKoffer.KofferStages.IDLE:
                // shouldn't even be in here
                break;
            case PianoRollTLKoffer.KofferStages.COUNTINPB:
                break;
            case PianoRollTLKoffer.KofferStages.PLAYBACK:
                break;
            case PianoRollTLKoffer.KofferStages.COUNTINRR:
                break;
            case PianoRollTLKoffer.KofferStages.RHYTHMREPEAT:
                break;
            case PianoRollTLKoffer.KofferStages.END:
                break;
        }
    }

    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (_timer.previewBeat == 1) _spawner.SpawnLine(_timeline.bpm, 1);
        if (_timer.previewBeat == 3) _spawner.SpawnLine(_timeline.bpm, 2);
        if (_timer.previewBeat == 5) _spawner.SpawnLine(_timeline.bpm, 3);
        if (_timer.previewBeat == 7) _spawner.SpawnLine(_timeline.bpm, 4);
    }
}
