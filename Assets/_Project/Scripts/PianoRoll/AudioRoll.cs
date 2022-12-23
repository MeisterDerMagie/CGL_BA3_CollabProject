using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioRoll : MonoBehaviour
{
    [SerializeField] private EventReference[] sound;

    private List<FMOD.Studio.EventInstance> instance;

    // MISSING: set the Event References when entering the scene via the soundID stuff

    public void SetUpAllInstances()
    {
        instance = new List<FMOD.Studio.EventInstance>();

        for (int i = 0; i < sound.Length; i++)
        {
            FMOD.Studio.EventInstance inst;

            inst = RuntimeManager.CreateInstance(sound[i]);
            inst.start();
            inst.setPaused(true);

            instance.Add(inst);
        }
    }

    // play sound when hitting a key
    public void PlayerInputSound(int soundID)
    {
        instance[soundID].setPaused(false);
        instance[soundID].release();

        instance[soundID] = RuntimeManager.CreateInstance(sound[soundID]);
        instance[soundID].start();
        instance[soundID].setPaused(true);
    }

    public void PlaySound(int sample)
    {
        RuntimeManager.PlayOneShot(sound[sample]);
    }
}
