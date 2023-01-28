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
    [SerializeField] private EventReference charSwish;

    public void PlayCrowd()
    {
        crowdInstance = RuntimeManager.CreateInstance(crowd);
        crowdInstance.start();
    }

    public void StopCrowd()
    {
        crowdInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED || state == FMOD.Studio.PLAYBACK_STATE.STOPPING) return;
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

    public void PlayCatchPhrase(EventReference catchphrase)
    {
        RuntimeManager.PlayOneShot(catchphrase);
    }

    public void PlayCharSwish()
    {
        RuntimeManager.PlayOneShot(charSwish);
    }
}
