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

    public void HidePlayers()
    {
        GameObject[] playerVisuals = FindPlayerVisuals();

        foreach (GameObject playerVisual in playerVisuals)
        {
            playerVisual.SetActive(false);
        }
    }

    public void ShowPlayers()
    {
        GameObject[] playerVisuals = FindPlayerVisuals();

        foreach (GameObject playerVisual in playerVisuals)
        {
            playerVisual.SetActive(true);
        }
    }

    private GameObject[] FindPlayerVisuals()
    {
        return GameObject.FindGameObjectsWithTag("PlayerVisuals");
    }
    
    private enum Visibility
    {
        DontChange,
        Show,
        Hide
    }
}
