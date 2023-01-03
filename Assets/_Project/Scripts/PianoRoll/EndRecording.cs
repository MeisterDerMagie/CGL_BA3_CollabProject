using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        // create random bar
        foreach (Eighth eighth in _recordInput.recordedBar)
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

            randomBar.Add(newEighth);
        }
    }
}
