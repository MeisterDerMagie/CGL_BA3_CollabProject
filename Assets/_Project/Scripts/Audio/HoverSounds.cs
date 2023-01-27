using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class HoverSounds : MonoBehaviour
{
    [SerializeField] private EventReference sound;
    private FMOD.Studio.EventInstance _instance;

    public void StartSound()
    {
        _instance = RuntimeManager.CreateInstance(sound);
        _instance.start();
    }

    public void StopSound()
    {
        // elfenbeinstein CHANGE --> fade out properly
        _instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _instance.release();
    }
}
