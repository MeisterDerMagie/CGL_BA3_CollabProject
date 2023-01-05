using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

/// <summary>
/// The Audio Roll plays all player instrument sounds via RecordInput and the different Piano Roll scripts.
/// There's two ways of playing audio. PlaySound uses FMOD's PlayOneShot function to play a sound from the piano roll whenever a recorded bar's eighth contains a note.
/// We found that the audio latency is too much to have during actual key input by the player.
/// So for the player instruments we're pre-loading all instances of the current instruments (max of 4), starting and immediately pausing them
/// Once the player presses the key, we unpause, release and load a new instance
/// </summary>

public class AudioRoll : MonoBehaviour
{
    private List<FMOD.Studio.EventInstance> instance;

    //[SerializeField] private EventReference[] events; // for testing locally

    public void SetUpAllInstances()
    {
        instance = new List<FMOD.Studio.EventInstance>();

        // for every instrumentID in local Player Data, create instance, start and pause immediately then add to list of instances.
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

    // When hitting a key: unpause sound and release instance; then create a new instance in its place, start and pause immediately
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

    /* for testing locally
    public void TestSound(int instrumentID)
    {
        if (instrumentID > events.Length - 1) instrumentID = 0;

        instance[instrumentID].setPaused(false);
        instance[instrumentID].release();

        instance[instrumentID] = RuntimeManager.CreateInstance(events[instrumentID]);
        instance[instrumentID].start();
        instance[instrumentID].setPaused(true);
    }

    public void TestSetup()
    {
        instance = new List<FMOD.Studio.EventInstance>();

        for (int i = 0; i < 4; i++)
        {
            FMOD.Studio.EventInstance inst;
            
            inst = RuntimeManager.CreateInstance(events[i]);
            inst.start();
            inst.setPaused(true);

            instance.Add(inst);
        }
    }
    */
}
