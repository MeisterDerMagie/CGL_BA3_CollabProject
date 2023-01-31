using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMainTheme : MonoBehaviour
{
    [SerializeField] private bool playAmbience = false;
    [SerializeField] private bool stopAmbience = true;
    [SerializeField] private bool stopMusic = true;
    [SerializeField] private bool playMusic = true;

    private void Start()
    {
        if (stopAmbience) PersistentAudioManager.Singleton.FadeOutAmbience();
        if (stopMusic) PersistentAudioManager.Singleton.FadeOutMainTheme();
    }

    public void FadeInAmbienceAndMainTheme()
    {
        if (playAmbience) PersistentAudioManager.Singleton.FadeInAmbience();
        if (playMusic) PersistentAudioManager.Singleton.FadeInMainTheme();
    }
}
