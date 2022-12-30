using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Playback : MonoBehaviour
{
    public List<PlayerData> playerDatas;
    
    private void Start()
    {
        playerDatas = FindObjectsOfType<PlayerData>().ToList();
        //playerDatas[0].Recording

        // start playback
    }
}
