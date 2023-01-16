using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PianoRollTLKoffer : MonoBehaviour
{
    public enum KofferStages
    {
        IDLE,
        COUNTINPB,
        PLAYBACK,
        COUNTINRR,
        RHYTHMREPEAT,
        END
    }

    // scripts
    [SerializeField] private CharDisplayPB _display;
    private PianoRollTimer _timer;
    private AudioRoll _audioRoll;
    [SerializeField] private Light _light;

    [Space]
    public float bpm = 110f;
    [Tooltip("seconds after loading before start of scene")]
    [SerializeField] private float startTime = 2.5f;
    [Tooltip("amount of bars before first bar")]
    public int countInToPlayback = 4;
    [Tooltip("amount of bars between bars")]
    public int countInToRhythmRepeat = 4;
    [Tooltip("bars after everything finished before going to next screen")]
    public int fadeOut = 2;

    int amountPlaybackPlayers;
    int currentPlayer;
    int currentBar;
    int barTimer;

    KofferStages currentStage;

    public List<List<Eighth>> testPlayers;

    public List<PlayerData> _allPlayers;

    void Start()
    {
        // get references to all relevant scripts:
        _timer = GetComponent<PianoRollTimer>();
        _audioRoll = GetComponentInChildren<AudioRoll>();
        //_audioRoll.SetUpAllInstances();
        _audioRoll.TestSetup();

        // get all players in scene and sort according to creativity points
        //_allPlayers = FindObjectsOfType<PlayerData>().ToList();
        // MISSING sort list @Martina

        currentPlayer = 0;
        currentBar = 0;

        currentStage = KofferStages.IDLE;

        // FOR TESTING LOCALLY:
        WriteTestBars();

        StartCoroutine(WaitToStart());
    }


    private void WriteTestBars()
    {
        // FOR TESTING:
        testPlayers = new List<List<Eighth>>();

        for (int player = 0; player < 8; player++)
        {
            List<Eighth> recording = new List<Eighth>();

            for (int b = 0; b < Constants.RECORDING_LENGTH; b++)
            {
                for (int i = 0; i < 8; i++)
                {
                    Eighth newEighth = new Eighth();

                    int x = Random.Range(0, 2);

                    if (x == 0)
                    {
                        newEighth.contains = true;

                        int sound = Random.Range(0, 4);
                        //newEighth.instrumentID = PlayerData.LocalPlayerData.InstrumentIds[sound];
                        newEighth.instrumentID = sound;
                    }
                    else
                    {
                        newEighth.contains = false;
                        newEighth.instrumentID = -1;
                    }

                    recording.Add(newEighth);
                }
            }
            testPlayers.Add(recording);
        }
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(startTime);
        StartPlayback();
    }

    void StartPlayback()
    {
        // start backing track
        GetComponentInChildren<BackingTrack>().StartMusic();

        // reset variables:
        barTimer = _timer.timelineBar + 1;
        currentPlayer = 0;
        currentBar = 0;
        amountPlaybackPlayers = 1;

        // set current stage
        currentStage = KofferStages.COUNTINPB;

        // tell preview to start as well
        GetComponent<PianoRollPrevKoffer>().StartPlayback();
    }

    public void NextBeat()
    {
        if (currentStage == KofferStages.IDLE) return;

        if (_timer.timelineBeat == 1) UpdateStage();

        // if we're playing back (then we're playing back with audio --> play audio)
        if (currentStage == KofferStages.PLAYBACK)
        {
            // test version:
            if (testPlayers[currentPlayer][((currentBar * 8) + _timer.timelineBeat) - 1].contains)
                _audioRoll.TestSound(testPlayers[currentPlayer][((currentBar * 8) + _timer.timelineBeat) - 1].instrumentID);
        }
    }

    void UpdateStage()
    {
        switch (currentStage)
        {
            case KofferStages.IDLE:
                // shouldn't even be in here
                break;
            case KofferStages.COUNTINPB:
                // if timer reaches count in --> go to next stage, reset all variables (reset current bar + bar timer)
                if (_timer.timelineBar - barTimer >= countInToPlayback)
                {
                    currentStage = KofferStages.PLAYBACK;
                    barTimer = _timer.timelineBar;
                    currentBar = 0;
                }
                break;
            case KofferStages.PLAYBACK:
                currentBar++;
                if (currentBar >= Constants.RECORDING_LENGTH)
                {
                    // we're always playing back one player, so if current bar hits length of recording --> go to next stage
                    currentStage = KofferStages.COUNTINRR;
                    barTimer = _timer.timelineBar;
                }
                break;
            case KofferStages.COUNTINRR:
                // if the amount of bars passed is the amount of count in bars go to next stage and reset variables
                if (_timer.timelineBar - barTimer >= countInToRhythmRepeat)
                {
                    currentStage = KofferStages.RHYTHMREPEAT;
                    barTimer = _timer.timelineBar;
                    currentPlayer = 0;
                    currentBar = 0;
                }
                break;
            case KofferStages.RHYTHMREPEAT:
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
                        if (amountPlaybackPlayers >= testPlayers.Count) // elfenbeinstein CHANGE to player data
                        {
                            // if so, leave the rhythm section
                            currentStage = KofferStages.END;
                            barTimer = _timer.timelineBar;

                        }
                        // else go back to count in the playback session and reset variables
                        else
                        {
                            amountPlaybackPlayers++;
                            currentPlayer = amountPlaybackPlayers - 1;
                            currentBar = 0;
                            barTimer = _timer.timelineBar;

                            currentStage = KofferStages.COUNTINPB;
                        }
                    }
                }
                break;
            case KofferStages.END:
                break;
        }
    }
}
