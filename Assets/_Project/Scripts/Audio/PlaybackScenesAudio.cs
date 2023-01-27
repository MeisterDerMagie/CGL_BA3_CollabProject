using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlaybackScenesAudio : MonoBehaviour
{
    [SerializeField] private EventReference applause;
    [SerializeField] private EventReference crowd;
    [SerializeField] private EventReference drumroll;
    private FMOD.Studio.EventInstance crowdInstance;

    public void PlayCrowd()
    {
        crowdInstance = RuntimeManager.CreateInstance(crowd);
        crowdInstance.start();
    }

    public void StopCrowd()
    {
        crowdInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        crowdInstance.release();
    }

    public void PlayDrumRoll()
    {
        RuntimeManager.PlayOneShot(drumroll);
    }

    public void PlayApplause()
    {
        RuntimeManager.PlayOneShot(applause);
    }
}
