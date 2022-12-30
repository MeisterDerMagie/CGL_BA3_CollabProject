using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RecordingUI : NetworkBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI countInText;
    [SerializeField] private TextMeshProUGUI promptTextField;
    [SerializeField] private GameObject pianoRollObj;
    private RecordInput _recordInput;
    private PianoRoll _pianoRoll;

    public bool _audio;

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsServer) return;
        
        promptTextField.SetText(PlayerData.LocalPlayerData.AssignedPrompt);
    }

    private void Start()
    {
        // get and set relevant scripts:
        _recordInput = pianoRollObj.GetComponentInChildren<RecordInput>();
        //_recordInput._recordingUI = this;
        _pianoRoll = pianoRollObj.GetComponent<PianoRoll>();

        // deactivate count in text:
        countInText.gameObject.SetActive(false);
    }
    public void PlayButton()
    {
        _audio = !_audio;
        _pianoRoll.PlayRecording(true, _audio, true);
    }

    public void RecordButton()
    {
        _recordInput.RecordButton();
    }

    public void TrashButton()
    {
        _recordInput.DeleteButton();
        _pianoRoll.GetComponent<NoteSpawner>().DeleteActiveNotes();
    }

    public void UpdateCountIn(bool active, string text)
    {
        countInText.gameObject.SetActive(active);
        countInText.text = text;
    }
}
