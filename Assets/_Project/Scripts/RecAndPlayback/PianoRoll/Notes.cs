using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// should sit on all notes and quarter note lines in the piano roll
/// gets current bpm, length of piano roll and target x position from the Note Spawner
/// calculates duration of movement from this and moves notes with Vector3Lerp Movement in fixed duration
/// deletes game object at the end
/// </summary>

public class Notes : MonoBehaviour
{
    private NoteSpawner spawner;

    public GameObject visuals;
    public Sprite[] sprites;
    public SpriteRenderer _secondRenderer;
    public SpriteRenderer _objRenderer;
    public GameObject startLine;
    public GameObject fatLine;

    public bool isStartingLine;
    public bool isFatLine;

    public void TestNoteSetUp(float bpm, int beatLength, float _targetX, NoteSpawner script, int instrumentID, int beat = -1)
    {
        float duration = (60f / bpm / 2f) * (beatLength);

        // choose button bg
        if (instrumentID >= sprites.Length) instrumentID = 0;
        _objRenderer.sprite = sprites[instrumentID];
        // deactivate instrument icon for testing
        _secondRenderer.gameObject.SetActive(false);

        spawner = script;

        // set target position on the right side of the Piano Roll
        Vector3 targetPos = new Vector3(_targetX, transform.localPosition.y, transform.localPosition.z);

        // start moving object in coroutine
        StartCoroutine(MoveToLeft(duration, targetPos));
    }

    public void NoteSetUp(float bpm, int beatLength, float _targetX, NoteSpawner script, int instrumentID, int beat = -1)
    {
        // instrumentID is set to -1 if it's a line being spawned
        // otherwise it is a note being spawned and used for setting the sprite

        // calculate total duration of travelling length of the piano roll in seconds
        // duration of eighth note = 60 seconds / bpm / 2
        // beat length = length of piano roll measured in eighth notes
        // --> total duration is eighth note duration times beat length
        float duration = (60f / bpm / 2f) * (beatLength);

        // instrumentID is only -1 when it is a line being spawned and then number is the current beat
        // so if a line is being spawned
        if (instrumentID == -1)
        {
            // if the current beat is 2 or 4, make the line opaque
            if (beat == 2 || beat == 4)
                MakeOpaque();
        }
        else
        {
            // convert instrumentID to line to chose correct button background:
            int line = 0;
            for (int i = 0; i < PlayerData.LocalPlayerData.InstrumentIds.Count; i++)
            {
                if (instrumentID == PlayerData.LocalPlayerData.InstrumentIds[i])
                    line = i;
            }
            if (line >= sprites.Length) line = 0;
            _objRenderer.sprite = sprites[line];


            // set icon image on top of background:
            if (_secondRenderer != null)
            {
                if (InstrumentsManager.Instance != null)
                {
                    // get correct sprite from Instruments Manager
                    Sprite sprite = InstrumentsManager.Instance.GetInstrument(PlayerData.LocalPlayerData.InstrumentIds[instrumentID]).instrumentIcon;

                    _secondRenderer.gameObject.SetActive(true);
                    _secondRenderer.sprite = sprite;
                }
            }
        }

        spawner = script;

        // set target position on the right side of the Piano Roll
        Vector3 targetPos = new Vector3(_targetX, transform.localPosition.y, transform.localPosition.z);

        // start moving object in coroutine
        StartCoroutine(MoveToLeft(duration, targetPos));
    }

    IEnumerator MoveToLeft(float duration, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = transform.localPosition;

        // move note towards target Pos with a fixed duration of movement by using Vector3.Lerp
        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        spawner.RemoveFromList(this.gameObject);
        Destroy(this.gameObject);
    }

    public void MakeOpaque()
    {
        if (_objRenderer == null) _objRenderer = GetComponent<SpriteRenderer>();
        Color c = _objRenderer.color;
        c.a = 0.3f;
        _objRenderer.color = c;
    }

    // used to set the lines active and inactive during the recording stage
    public void Activate(bool value)
    {
        if (value)
        {
            if (isStartingLine && startLine != null) startLine.SetActive(true);
            else if (isFatLine && fatLine != null) fatLine.SetActive(true);
            else visuals.SetActive(true);
        }
        else
        {
            if (startLine != null) startLine.SetActive(false);
            if (fatLine != null) fatLine.SetActive(false);
            visuals.SetActive(false);
        }
    }

    // used during recording stage to make the beginning and end of the bar clearly visible 
    public void StartLine(bool value)
    {
        if (startLine == null) return;
        startLine.SetActive(value);
        if (fatLine != null) fatLine.SetActive(!value);
        visuals.SetActive(!value);
    }

    public void FatLine(bool value)
    {
        if (fatLine == null) return;
        fatLine.SetActive(value);
        if (startLine != null) startLine.SetActive(!value);
        visuals.SetActive(!value);
        isFatLine = value;
    }
}
