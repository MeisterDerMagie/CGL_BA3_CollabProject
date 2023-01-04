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
    }

    public void PlaySound(int instrumentID)
    {
        if (InstrumentsManager.Instance == null) return;
        RuntimeManager.PlayOneShot(InstrumentsManager.Instance.GetInstrument(instrumentID).soundEvent);
    }
}
