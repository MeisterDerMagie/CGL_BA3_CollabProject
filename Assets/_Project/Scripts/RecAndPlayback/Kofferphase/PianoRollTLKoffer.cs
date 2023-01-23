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
    private PlayerInputRR _playerInput;
    [SerializeField] private KofferUI _ui;

    [Space]
    public float bpm = 110f;
    [Tooltip("seconds after loading before start of scene")]
    [SerializeField] private float startTime = 2.5f;
    [Tooltip("Duration in seconds, how long first schubidu moderation lasts")]
    [SerializeField] private float schubiduTime = 2.5f;
    [Tooltip("amount of bars before audio bar")]
    public int countInToPlayback = 4;
    [Tooltip("amount of bars before rhythm repeat")]
    public int countInToRhythmRepeat = 4;
    [Tooltip("bars after everything finished before going to next screen")]
    public int fadeOut = 2;

    int amountPlaybackPlayers;
    public int currentPlayer;
    int currentBar;
    int barTimer;

    KofferStages currentStage;

    public List<List<Eighth>> testPlayers;

    public List<PlayerData> sortedPlayers;

    public bool testLocally;
    public int testPlayerAmount;
    public bool counterPlayback = true;

    Eighth empty;

    void Start()
    {
        // get references to all relevant scripts:
        _timer = GetComponent<PianoRollTimer>();
        _audioRoll = GetComponentInChildren<AudioRoll>();
        if (testLocally) _audioRoll.TestSetup();
        else _audioRoll.SetUpAllInstances();
        _playerInput = GetComponentInChildren<PlayerInputRR>();
        _playerInput.scorePlayability = false;

        // deactivate player input + set ui:
        SetPlayerInput(false);
        if (!testLocally)  _ui.SetDisplayToSelf();

        currentPlayer = 0;
        currentBar = 0;

        currentStage = KofferStages.IDLE;

        // FOR TESTING LOCALLY:
        if (testLocally) WriteTestBars();
        else
        {
            // get all players in scene and sort according to creativity points
            sortedPlayers = new List<PlayerData>();
            List<PlayerData> allPlayers = new List<PlayerData>();
            allPlayers = FindObjectsOfType<PlayerData>().ToList();
            sortedPlayers = allPlayers.OrderByDescending(allPlayers => allPlayers.PointsCreativity).ToList();
        }

        StartCoroutine(WaitForSchubidu());

        empty = new Eighth();
        empty.contains = false;
        empty.instrumentID = -1;
    }


    private void WriteTestBars()
    {
        // FOR TESTING:
        testPlayers = new List<List<Eighth>>();

        for (int player = 0; player < testPlayerAmount; player++)
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

    IEnumerator WaitForSchubidu()
    {
        yield return new WaitForSeconds(startTime);

        // SCHUBIDU anfangen
        _ui.Schubidu(0, true);
        // MISSING: Trommelwirbel

        StartCoroutine(WaitToStart());
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(schubiduTime);

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

        // stop player input (should already be inactive, just in case)
        SetPlayerInput(false);
        _playerInput.SetUpRecording(bpm, this);

        // tell preview to start as well
        GetComponent<PianoRollPrevKoffer>().StartPlayback();

        // set up Schubidu:
        _ui.Schubidu(1, true);

        // display character and prompt
        if (!testLocally) _ui.SetDisplay(sortedPlayers[currentPlayer]);
    }

    public void NextBeat()
    {
        if (currentStage == KofferStages.IDLE) return;

        if (_timer.timelineBeat == 1) UpdateStage();

        // set count in text if we're in last bar before the playback or rhythm repeat stage
        if ((currentStage == KofferStages.COUNTINPB && _timer.timelineBar - barTimer >= countInToPlayback - 1 && counterPlayback) || (currentStage == KofferStages.COUNTINRR && _timer.timelineBar - barTimer >= countInToRhythmRepeat - 1))
        {
            if (_timer.timelineBeat == 1) _ui.CountInText("4");
            else if (_timer.timelineBeat == 3) _ui.CountInText("3");
            else if (_timer.timelineBeat == 5) _ui.CountInText("2");
            else if (_timer.timelineBeat == 7) _ui.CountInText("1");

            // if in count in to rr dann feedback anmachen + start recording
            if (currentStage == KofferStages.COUNTINRR)
            {
                _playerInput.StartRecording();
                // MISSING: feedback anmachen (oder ist evtl über start rec schon gelöst)
            }
        }

        // if we're amout to start playing the last Player's bar --> start scoring for accuracy
        if (currentStage == KofferStages.RHYTHMREPEAT && _timer.timelineBeat == 8 && currentPlayer == amountPlaybackPlayers - 2)
            _playerInput.AccuracyStart(bpm, currentPlayer + 1);

        // if we're playing back (then we're playing back with audio --> play audio)
        if (currentStage == KofferStages.PLAYBACK)
        {
            // test version:
            if (testLocally)
            {
                if (testPlayers[currentPlayer][((currentBar * 8) + _timer.timelineBeat) - 1].contains)
                    _audioRoll.TestSound(testPlayers[currentPlayer][((currentBar * 8) + _timer.timelineBeat) - 1].instrumentID);
            }
            else
            {
                if (sortedPlayers[currentPlayer].Recording[((currentBar * 8) + _timer.timelineBeat) - 1].contains)
                    _audioRoll.PlayerInputSound(sortedPlayers[currentPlayer].Recording[((currentBar * 8) + _timer.timelineBeat) - 1].instrumentID);
            }
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
                    #region START PLAYBACK STAGE
                    currentStage = KofferStages.PLAYBACK;
                    barTimer = _timer.timelineBar;
                    currentBar = 0;

                    // stop count In text
                    _ui.CountInText("");
                    _ui.TurnOnLight(true);

                    // stop schubidu:
                    _ui.Schubidu(-1);
                    #endregion
                }
                break;
            case KofferStages.PLAYBACK:
                currentBar++;
                if (currentBar >= Constants.RECORDING_LENGTH)
                {
                    #region START COUNT IN TO RHYTHM REPEAT
                    // we're always playing back one player, so if current bar hits length of recording --> go to next stage
                    currentStage = KofferStages.COUNTINRR;
                    barTimer = _timer.timelineBar;

                    // set player input active when going into the count in to rhythm repeat
                    SetPlayerInput(true);
                    _ui.RecFrame(true);
                    _ui.TurnOnLight(false);
                    if (amountPlaybackPlayers == 1) _playerInput.scorePlayability = true;

                    // start schubidu:
                    _ui.Schubidu(amountPlaybackPlayers, false);

                    // reset display to self
                    if (!testLocally)  _ui.SetDisplayToSelf();
                    #endregion
                }
                break;
            case KofferStages.COUNTINRR:
                // if the amount of bars passed is the amount of count in bars go to next stage and reset variables
                if (_timer.timelineBar - barTimer >= countInToRhythmRepeat)
                {
                    #region START RHYTHM REPEAT STAGE
                    currentStage = KofferStages.RHYTHMREPEAT;
                    barTimer = _timer.timelineBar;
                    currentPlayer = 0;
                    currentBar = 0;

                    // stop count In text
                    _ui.CountInText("");
                    _ui.TurnOnLight(true);

                    // turn off schubidu:
                    _ui.Schubidu(-1);

                    // set prompt text
                    if (!testLocally) _ui.PromptText(sortedPlayers[currentPlayer].AssignedPrompt);
                    #endregion
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
                        if ((amountPlaybackPlayers >= testPlayers.Count && testLocally) || (!testLocally && amountPlaybackPlayers >= sortedPlayers.Count))
                        {
                            #region GO TO END
                            // if so, leave the rhythm section
                            currentStage = KofferStages.END;
                            barTimer = _timer.timelineBar;
                            
                            _ui.TurnOnLight(false);
                            _ui.PromptText("We're done now, matey!");
                            _ui.Schubidu(9);

                            GetComponentInChildren<AccuracyScoring>().SendToServer();
                            #endregion

                        }
                        // else go back to count in the playback session and reset variables
                        else
                        {
                            #region START COUNT IN PLAYBACK
                            amountPlaybackPlayers++;
                            currentPlayer = amountPlaybackPlayers - 1;
                            currentBar = 0;
                            barTimer = _timer.timelineBar;

                            currentStage = KofferStages.COUNTINPB;

                            _ui.PromptText("");

                            // schubidu count in moderation:
                            _ui.Schubidu(amountPlaybackPlayers);

                            // display character and prompt
                            if (!testLocally) _ui.SetDisplay(sortedPlayers[currentPlayer]);
                            #endregion
                        }

                        // either way, stop recording and stop player input:
                        SetPlayerInput(false);
                        _ui.RecFrame(false);
                        _playerInput.StopRecording();
                        _ui.TurnOnLight(false);
                        _playerInput.scorePlayability = false;
                    }
                    else
                    {
                        // set prompt to next player:
                        if (!testLocally) _ui.PromptText(sortedPlayers[currentPlayer].AssignedPrompt);
                    }
                }
                break;
            case KofferStages.END:
                if (_timer.timelineBar - barTimer >= fadeOut)
                {
                    Debug.Log("end of koffer stage");
                }
                break;
        }
    }

    void Schubidu()
    {

    }

    void SetPlayerInput(bool _active)
    {
        _playerInput.active = _active;
        _ui.GreyOutDJPult(!_active);
    }
}
