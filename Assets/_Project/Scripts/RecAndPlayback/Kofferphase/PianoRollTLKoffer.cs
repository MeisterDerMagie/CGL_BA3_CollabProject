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

        // get all players in scene and sort according to creativity points
        //_allPlayers = FindObjectsOfType<PlayerData>().ToList();
        // MISSING sort list @Martina

        currentPlayer = 0;
        currentBar = 0;

        currentStage = KofferStages.IDLE;

        // FOR TESTING:
        WriteTestBars();
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
        barTimer = _timer.timelineBar;
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

        if (_timer.timelineBeat == 1)
        {
            UpdateStage();
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
                break;
            case KofferStages.PLAYBACK:
                break;
            case KofferStages.COUNTINRR:
                break;
            case KofferStages.RHYTHMREPEAT:
                break;
            case KofferStages.END:
                break;
        }
    }
}
