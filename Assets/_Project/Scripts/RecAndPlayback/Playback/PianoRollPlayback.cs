using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PianoRollPlayback : MonoBehaviour
{
    [SerializeField] private CharDisplayPB _display;

    public List<PlayerData> playerDatas;
    int playerCount;
    int currentPlayer;
    
    private void Start()
    {
        playerDatas = FindObjectsOfType<PlayerData>().ToList();
        playerCount = playerDatas.Count;
        currentPlayer = 0;

        //playerDatas[0].Recording

        // start playback
    }
}


// start Playback

// for every player in the game

// display prompt
// display player icon
// display player name
// turn on light, turn off light etc

// play recording of player
// count in 2 - 3 bars (display) maybe with enum of playback stages IDLE, COUNT-IN, PLAYING BACK, IDLE
// play bar audio + visuals
// possibly play applause
// go to next player or finish

// if done --> stop music, waiting screen, go to voting stage