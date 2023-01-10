using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// all buttons during the recording stage handled via this script
/// sets up the count in text + prompt field
/// </summary>

public class RecordingUI : NetworkBehaviour
{
    [Tooltip("0 is play, 1 is stop")]
    [SerializeField] private Sprite[] playButtonImgs;
    [SerializeField] private Image playButtonRend;
    [SerializeField] private TMPro.TextMeshProUGUI countInText;
    [SerializeField] private TextMeshProUGUI promptTextField;
    [SerializeField] private GameObject pianoRollObj;
    private RecordInput _recordInput;
    private PianoRollRecording _pianoRoll;

    public bool playingBack;
    public bool playButtonActive;

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsServer) return;
        
        // set the prompt to the player's assigned prompt
        promptTextField.SetText(PlayerData.LocalPlayerData.AssignedPrompt);
        playButtonActive = true;
    }

    private void Start()
    {
        // get and set relevant scripts:
        _recordInput = pianoRollObj.GetComponentInChildren<RecordInput>();
        _pianoRoll = pianoRollObj.GetComponent<PianoRollRecording>();

        // deactivate count-in text:
        countInText.gameObject.SetActive(false);
    }
    public void PlayButton()
    {
        if (!playButtonActive) return;

        playingBack = !playingBack;

        if (playingBack) _pianoRoll.ControlPlayback(PianoRollRecording.RecPBStage.WAITFORPB);
        else _pianoRoll.ControlPlayback(PianoRollRecording.RecPBStage.INACTIVE);

        SetPlayButton();
    }

    public void SetPlayButton()
    {
        if (playingBack) playButtonRend.sprite = playButtonImgs[1];
        else playButtonRend.sprite = playButtonImgs[0];
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
