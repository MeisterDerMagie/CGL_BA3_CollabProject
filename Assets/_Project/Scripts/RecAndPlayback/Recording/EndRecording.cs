using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

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
    [SerializeField] private LoadNextSceneWhenAllClientsAreDone _loadNext;
    [SerializeField] private WaitingScreenController_Generic _waitScreen;

    [SerializeField] List<Button> buttons;

    List<Eighth> randomBar;

    void Start()
    {
        _recordInput = GetComponent<RecordInput>();
    }

    public void EndRecordingStage()
    {
        if (NetworkManager.Singleton.IsServer) return;

        // stop player instruments:
        if (_recordInput == null) _recordInput = GetComponent<RecordInput>();
        _recordInput.stageEnded = true;

        // send over the recorded bar or a random bar
        SendRecordingToServer();

        // deactivate music
        GetComponentInParent<PianoRollRecording>().StopMusic();

        // deactivate all buttons
        foreach (Button b in buttons)
        {
            b.interactable = false;
        }

        // activate waiting screen and set as done
        _waitScreen.Ready();
        _waitScreen.Show();
        _loadNext.Done();

    }
    void SendRecordingToServer()
    {
        bool empty = true;

        // check if the recording is completely empty
        for (int b = 0; b < Constants.RECORDING_LENGTH; b++)
            foreach (Eighth eighth in _recordInput.recording[b])
                if (eighth.contains) empty = false;

        if (empty)
        {
            CreateRandomBar();
            PlayerData.LocalPlayerData.SetRecording(randomBar);
        }
        else
        {
            List<Eighth> _recording = new List<Eighth>();

            for (int b = 0; b < Constants.RECORDING_LENGTH; b++)
                foreach (Eighth eighth in _recordInput.recording[b])
                    _recording.Add(eighth);

            PlayerData.LocalPlayerData.SetRecording(_recording);
        }
    }

    void CreateRandomBar()
    {
        randomBar = new List<Eighth>();

        // create random bar; for every eighth randomly create bool true/false and if true, random range which instrumentID
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
}
