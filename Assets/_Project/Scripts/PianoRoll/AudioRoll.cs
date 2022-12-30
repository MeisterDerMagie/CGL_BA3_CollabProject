using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioRoll : MonoBehaviour
{
    //[SerializeField] private EventReference[] sound;

    private List<FMOD.Studio.EventInstance> instance;
    private List<EventReference> eventsFMOD;

    public void SetUpAllInstances()
    {
        /* --- obsolete, version pre networking stuff
        instance = new List<FMOD.Studio.EventInstance>();

        for (int i = 0; i < sound.Length; i++)
        {
            FMOD.Studio.EventInstance inst;

            inst = RuntimeManager.CreateInstance(sound[i]);
            inst.start();
            inst.setPaused(true);

            instance.Add(inst);
        }
        */

        instance = new List<FMOD.Studio.EventInstance>();
        eventsFMOD = new List<EventReference>();

        for (int i = 0; i < PlayerData.LocalPlayerData.InstrumentIds.Count; i++)
        {
            FMOD.Studio.EventInstance inst;
            int ID = PlayerData.LocalPlayerData.InstrumentIds[i];

            inst = RuntimeManager.CreateInstance(InstrumentsManager.Instance.GetInstrument(ID).soundEvent);
            inst.start();
            inst.setPaused(true);

            instance.Add(inst);
        }
    }

    // play sound when hitting a key
    public void PlayerInputSound(int keyID)
    {
        instance[keyID].setPaused(false);
        instance[keyID].release();

        int ID = PlayerData.LocalPlayerData.InstrumentIds[keyID];
        instance[keyID] = RuntimeManager.CreateInstance(InstrumentsManager.Instance.GetInstrument(ID).soundEvent);
        instance[keyID].start();
        instance[keyID].setPaused(true);


        /*
        instance[soundID].setPaused(false);
        instance[soundID].release();

        instance[soundID] = RuntimeManager.CreateInstance(sound[soundID]);
        instance[soundID].start();
        instance[soundID].setPaused(true);
        */
    }

    public void PlaySound(int instrumentID)
    {
        RuntimeManager.PlayOneShot(InstrumentsManager.Instance.GetInstrument(instrumentID).soundEvent);
    }
}
