using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BackingTrack : MonoBehaviour
{
    [SerializeField] private EventReference track;
    private FMOD.Studio.EventInstance musicInstance;

    public void StartMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(track);
        musicInstance.start();
    }

    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
