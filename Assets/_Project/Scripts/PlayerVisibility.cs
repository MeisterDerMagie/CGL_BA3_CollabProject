using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisibility : MonoBehaviour
{
    [SerializeField] private Visibility visibilityOnStart;

    private void Start()
    {
        switch (visibilityOnStart)
        {
            case Visibility.DontChange:
                break;
            case Visibility.Show:
                ShowPlayers();
                break;
            case Visibility.Hide:
                HidePlayers();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void HidePlayers()
    {
        Visuals[] playerVisuals = FindPlayerVisuals();

        foreach (Visuals playerVisual in playerVisuals)
        {
            playerVisual.gameObject.SetActive(false);
        }
    }

    public static void ShowPlayers()
    {
        Visuals[] playerVisuals = FindPlayerVisuals();

        foreach (Visuals playerVisual in playerVisuals)
        {
            playerVisual.gameObject.SetActive(true);
        }
    }

    private static Visuals[] FindPlayerVisuals()
    {
        return GameObject.FindObjectsOfType<Visuals>(true);
        //return GameObject.FindGameObjectsWithTag("PlayerVisuals");
    }
    
    private enum Visibility
    {
        DontChange,
        Show,
        Hide
    }
}
