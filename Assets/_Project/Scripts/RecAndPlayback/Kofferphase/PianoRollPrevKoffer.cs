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
        barTimer = _timer.previewBar - 1; // might have to adjust +- 1
        currentPlayer = 0;
        currentBar = 0;
        amountPlaybackPlayers = 1;

        // set current stage
        currentStage = PianoRollTLKoffer.KofferStages.COUNTINPB;
    }

    public void NextBeat()
    {
        if (currentStage == PianoRollTLKoffer.KofferStages.IDLE || currentStage == PianoRollTLKoffer.KofferStages.END) return;

        if (_timer.previewBeat == 1) UpdateStage();

        PlayQuarterNote();

        // if we're playing back or repeating rhythm --> spawn notes
        if (currentStage == PianoRollTLKoffer.KofferStages.RHYTHMREPEAT || currentStage == PianoRollTLKoffer.KofferStages.PLAYBACK)
        {
            // with test players:
            if (_timeline.testLocally)
            {
                if (_timeline.testPlayers[currentPlayer][((currentBar * 8) + _timer.previewBeat) - 1].contains)
                    _spawner.SpawnTestNote(_timeline.testPlayers[currentPlayer][((currentBar * 8) + _timer.previewBeat) - 1].instrumentID, _timeline.bpm);
            }
            else
            {
                if (_timeline.sortedPlayers[currentPlayer].Recording[((currentBar * 8) + _timer.previewBeat) - 1].contains)
                    _spawner.SpawnNote(_timeline.sortedPlayers[currentPlayer].Recording[((currentBar * 8) + _timer.previewBeat) - 1].instrumentID, _timeline.bpm);
            }
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
                // if timer reaches count in --> go to next stage, reset all variables (reset current bar + bar timer)
                if (_timer.previewBar - barTimer >= _timeline.countInToPlayback)
                {
                    currentStage = PianoRollTLKoffer.KofferStages.PLAYBACK;
                    barTimer = _timer.previewBar;
                    currentBar = 0;
                }
                break;
            case PianoRollTLKoffer.KofferStages.PLAYBACK:
                currentBar++;
                if (currentBar >= Constants.RECORDING_LENGTH)
                {
                    // we're always playing back one player, so if current bar hits length of recording --> go to next stage
                    currentStage = PianoRollTLKoffer.KofferStages.COUNTINRR;
                    barTimer = _timer.previewBar;
                }
                break;
            case PianoRollTLKoffer.KofferStages.COUNTINRR:
                // if the amount of bars passed is the amount of count in bars go to next stage and reset variables
                if (_timer.previewBar - barTimer >= _timeline.countInToRhythmRepeat)
                {
                    currentStage = PianoRollTLKoffer.KofferStages.RHYTHMREPEAT;
                    barTimer = _timer.previewBar;
                    currentPlayer = 0;
                    currentBar = 0;
                }
                break;
            case PianoRollTLKoffer.KofferStages.RHYTHMREPEAT:
                // go to next bar
                currentBar++;
                // if next bar is greater than length of recording
                if (currentBar >= Constants.RECORDING_LENGTH)
                {
                    // go to next player and reset bar
                    currentPlayer++;
                    currentBar = 0;

                    // if the next player is greater than the total amount of players we're currently playing back
                    if (currentPlayer >= amountPlaybackPlayers)
                    {
                        // check if amount of Playback players is greater than total amount of players in scene
                        if (_timeline.testLocally)
                        {
                            if (amountPlaybackPlayers >= _timeline.testPlayers.Count)
                            {
                                // if so, leave the rhythm section
                                currentStage = PianoRollTLKoffer.KofferStages.END;
                                barTimer = _timer.previewBar;

                            }
                            // else go back to count in the playback session and reset variables
                            else
                            {
                                amountPlaybackPlayers++;
                                currentPlayer = amountPlaybackPlayers - 1;
                                currentBar = 0;
                                barTimer = _timer.previewBar;

                                currentStage = PianoRollTLKoffer.KofferStages.COUNTINPB;
                            }
                        }
                        else
                        {
                            if (amountPlaybackPlayers >= _timeline.sortedPlayers.Count)
                            {
                                // if so, leave the rhythm section
                                currentStage = PianoRollTLKoffer.KofferStages.END;
                                barTimer = _timer.previewBar;

                            }
                            // else go back to count in the playback session and reset variables
                            else
                            {
                                amountPlaybackPlayers++;
                                currentPlayer = amountPlaybackPlayers - 1;
                                currentBar = 0;
                                barTimer = _timer.previewBar;

                                currentStage = PianoRollTLKoffer.KofferStages.COUNTINPB;
                            }
                        }
                    }
                }
                break;
            case PianoRollTLKoffer.KofferStages.END:
                break;
        }
    }

    void PlayQuarterNote()
    {
        // only spawn lines on 1s and 3s, so on first and fifth eighth
        if (_timer.previewBeat == 1)
        {
            if (currentStage == PianoRollTLKoffer.KofferStages.PLAYBACK && currentBar == 0)
                _spawner.SpawnLine(_timeline.bpm, 1, true);
            else _spawner.SpawnLine(_timeline.bpm, 1);
        }
        if (_timer.previewBeat == 3) _spawner.SpawnLine(_timeline.bpm, 2);
        if (_timer.previewBeat == 5) _spawner.SpawnLine(_timeline.bpm, 3);
        if (_timer.previewBeat == 7) _spawner.SpawnLine(_timeline.bpm, 4);
    }
}
