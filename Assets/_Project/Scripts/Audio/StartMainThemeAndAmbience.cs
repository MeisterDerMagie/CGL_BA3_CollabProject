//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMainThemeAndAmbience : MonoBehaviour
{
    private void Start()
    {
        if (Unity.Netcode.NetworkManager.Singleton != null && Unity.Netcode.NetworkManager.Singleton.IsServer) return;
        PersistentAudioManager.Singleton.StartMainTheme();
        PersistentAudioManager.Singleton.StartAmbience();
    }
}