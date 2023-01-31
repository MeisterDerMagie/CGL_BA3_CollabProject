using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TestAudio : MonoBehaviour
{
    public List<KeyCode> inputs;
    public List<EventReference> events;

    void Update()
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            if (Input.GetKeyDown(inputs[i])) RuntimeManager.PlayOneShot(events[i]);
        }
    }
}
