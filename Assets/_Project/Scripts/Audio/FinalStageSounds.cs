using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FinalStageSounds : MonoBehaviour
{
    [SerializeField] private EventReference confetti;
    private EventInstance confettiInstance;
    [SerializeField] private EventReference celebration;
    private EventInstance celebrationInstance;

    void Start()
    {
        confettiInstance = RuntimeManager.CreateInstance(confetti);
        confettiInstance.start();

        celebrationInstance = RuntimeManager.CreateInstance(celebration);
        celebrationInstance.start();

        PersistentAudioManager.Singleton.FadeInAmbience();
        PersistentAudioManager.Singleton.FadeInMainTheme();
    }

    private void OnDestroy()
    {
        confettiInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        confettiInstance.release();

        celebrationInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        celebrationInstance.release();
    }
}
