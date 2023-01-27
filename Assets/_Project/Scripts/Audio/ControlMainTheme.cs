using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMainTheme : MonoBehaviour
{
    private void Start()
    {
        PersistentAudioManager.Singleton.FadeOutAmbience();
        PersistentAudioManager.Singleton.FadeOutMainTheme();
    }

    public void FadeInAmbienceAndMainTheme()
    {
        PersistentAudioManager.Singleton.FadeInAmbience();
        PersistentAudioManager.Singleton.FadeInMainTheme();
    }
}
