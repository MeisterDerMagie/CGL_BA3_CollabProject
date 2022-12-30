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
    public int sample;

    private NoteSpawner spawner;
    private AudioRoll _audioRoll;

    public Sprite[] sprites;
    public SpriteRenderer _secondRenderer;
    SpriteRenderer _objRenderer;

    public void NoteSetUp(float bpm, int beatLength, float _targetX, NoteSpawner script, AudioRoll audio, int s, int number = -1)
    {
        // calculate total duration of travelling length of the piano roll in seconds
        // duration of quarter note = 60 seconds / bpm
        // beat length = length of piano roll measured in quarter notes
        // --> total duration is quarter notes times beat length
        float duration = 60f / bpm * (beatLength);

        _objRenderer = GetComponent<SpriteRenderer>();

        // s is only -1 when it is a line being spawned and then it is the current beat
        if (s == -1)
        {
            //if (number != -1) _secondRenderer.sprite = sprites[number - 1];
            if (number == 2 || number == 4)
                MakeOpaque();
        }
        else
        {
            // set button bg to correct background
            _objRenderer.sprite = sprites[s];

            if (_secondRenderer != null)
            {
                // MISSING: set icon for note
                /*
                _secondRenderer.gameObject.SetActive(true);
                _secondRenderer.sprite = 
                */
            }
        }

        spawner = script;
        _audioRoll = audio;
        sample = s;

        // set target position on the right side of the Piano Roll
        Vector3 targetPos = new Vector3(_targetX, transform.localPosition.y, transform.localPosition.z);

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

        spawner.RemoveNote(this.gameObject);
        Destroy(this.gameObject);
    }

    public void MakeOpaque()
    {
        if (_objRenderer == null) _objRenderer = GetComponent<SpriteRenderer>();
        Color c = _objRenderer.color;
        c.a = 0.3f;
        _objRenderer.color = c;
    }
}