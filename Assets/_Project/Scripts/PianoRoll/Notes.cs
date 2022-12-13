using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// should sit on all notes and quarter note lines in the piano roll
/// gets current bpm, length of piano roll and target x position from the Piano roll
/// calculates duration of movement from this and moves notes with Vector3Lerp Movement in fixed duration
/// deletes game object at the end
/// </summary>

public class Notes : MonoBehaviour
{
    public void NoteSetUp(float bpm, int beatLength, float _targetX)
    {
        // calculate total duration of travelling length of the piano roll in seconds
        // duration of quarter note = 60 seconds / bpm
        // beat length = length of piano roll measured in quarter notes
        // --> total duration is quarter notes times beat length
        float duration = 60f / bpm * beatLength;

        // set target position on the right side of the Piano Roll
        Vector3 targetPos = new Vector3(_targetX, transform.position.y, transform.position.z);

        StartCoroutine(MoveToLeft(duration, targetPos));
    }

    IEnumerator MoveToLeft(float duration, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        // move note towards target Pos with a fixed duration of movement by using Vector3.Lerp
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        
        Destroy(this.gameObject);
    }
}
