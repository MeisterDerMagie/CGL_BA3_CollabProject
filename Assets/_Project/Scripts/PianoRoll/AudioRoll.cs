using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioRoll : MonoBehaviour
{
    [SerializeField] private EventReference[] sound;

    public void PlaySound(int sample)
    {
        RuntimeManager.PlayOneShot(sound[sample]);
    }
}
