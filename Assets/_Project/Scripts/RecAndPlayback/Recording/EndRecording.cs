using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// functionality for what happens at end of recording stage (when timer hit zero or player set ready):
/// stop key inputs from player
/// stop button interactability
/// send over the recording to server (or if the recording is empty create and send over a random bar)
/// stop the music
/// load the next scene
/// </summary>

public class EndRecording : MonoBehaviour
{
    RecordInput _recordInput;

    [SerializeField] List<Button> buttons;

    List<Eighth> randomBar;

    void Start()
    {
        _recordInput = GetComponent<RecordInput>();
    }

    public void EndRecordingStage()
    {
        // stop player instruments:
        GetComponent<RecordInput>().stageEnded = true;

        // send over the recorded bar or a random bar
        SendRecordingToServer();

        // deactivate music
        GetComponentInParent<PianoRollRecording>().StopMusic();

        // deactivate all buttons
        foreach (Button b in buttons)
        {
            b.interactable = false;
        }

        // activate waiting screen elfenbeinstein MISSING
    }
    void SendRecordingToServer()
    {
        bool empty = true;
        foreach (Eighth eighth in _recordInput.recordedBar)
        {
            if (eighth.contains) empty = false;
        }

        if (empty)
        {
            CreateRandomBar();
            PlayerData.LocalPlayerData.SetRecording(randomBar);
        }
        else
        {
            PlayerData.LocalPlayerData.SetRecording(_recordInput.recordedBar);
        }
    }

    void CreateRandomBar()
    {
        randomBar = new List<Eighth>();

        // create random bar; for every eighth randomly create bool true/false and if true, random range which instrumentID
        foreach (Eighth eighth in _recordInput.recordedBar)
        {
            Eighth newEighth = new Eighth();

            int x = Random.Range(0, 2);

            if (x == 0)
            {
                newEighth.contains = true;

                int sound = Random.Range(0, 4);
                newEighth.instrumentID = PlayerData.LocalPlayerData.InstrumentIds[sound];
            }
            else
            {
                newEighth.contains = false;
                newEighth.instrumentID = -1;
            }

            randomBar.Add(newEighth);
        }
    }
}
