using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioRoll : MonoBehaviour
{
    [SerializeField] private EventReference[] sound;

    // MISSING: set the Event References when entering the scene via the soundID stuff

    public void PlaySound(int sample)
    {
        RuntimeManager.PlayOneShot(sound[sample]);
    }
}
