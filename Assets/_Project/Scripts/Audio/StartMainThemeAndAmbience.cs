//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMainThemeAndAmbience : MonoBehaviour
{
    private void Start()
    {
        //PersistentAudioManager.Singleton.StartMainTheme();
        PersistentAudioManager.Singleton.StartAmbience();
    }
}