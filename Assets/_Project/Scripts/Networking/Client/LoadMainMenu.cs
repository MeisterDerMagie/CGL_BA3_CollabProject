using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using Wichtel.SceneManagement;

//Load the main menu on the client. Why delayed? Because we want to make sure that all systems are initialized before switching to the main menu.
public class LoadMainMenu : MonoBehaviour
{
    [SerializeField] private float delayInFrames = 5;
    [SerializeField] private SceneLoader mainMenuLoader;


    #if !UNITY_SERVER
    private void Start() => Timing.RunCoroutine(_LoadMainMenuDelayed());

    private IEnumerator<float> _LoadMainMenuDelayed()
    {
        for (int i = 0; i < delayInFrames; i++)
        {
            yield return Timing.WaitForOneFrame;
        }
        
        mainMenuLoader.Load();
    }
    #endif
}
