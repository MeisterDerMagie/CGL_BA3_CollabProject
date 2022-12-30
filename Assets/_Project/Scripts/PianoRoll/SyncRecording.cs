using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncRecording : MonoBehaviour
{
    RecordInput _recordInput;

    void Start()
    {
        _recordInput = GetComponent<RecordInput>();
    }

    public void SendRecordingToServer()
    {
        /*
        foreach (Eighth eighth in _recordInput.recordedBar)
        {

        }
        */
        PlayerData.LocalPlayerData.SetRecording(_recordInput.recordedBar);
    }
}
