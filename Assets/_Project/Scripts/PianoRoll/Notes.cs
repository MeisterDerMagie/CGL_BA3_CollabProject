using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    float targetX;


    public void NoteSetUp(float bpm, int beatLength, float _targetX)
    {
        targetX = _targetX;

        // total duration of travelling length of the piano roll
        // quarter note = bpm/60/4
        // beat length = length of piano roll measured in quarter notes
        float duration = bpm / 60f / 4f * beatLength;

        StartCoroutine(MoveToLeft(duration));
    }

    IEnumerator MoveToLeft(float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        Destroy(this.gameObject);
    }
}
