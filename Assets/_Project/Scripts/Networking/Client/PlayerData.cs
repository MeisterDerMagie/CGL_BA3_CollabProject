//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

public class PlayerData : NetworkBehaviour
{
    public string PlayerName => _playerName.Value.ToString();

    //Network Variables
    private NetworkVariable<FixedString128Bytes> _playerName;

    private void Awake()
    {
        _playerName = new NetworkVariable<FixedString128Bytes>(new FixedString128Bytes("Unknown Player"));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1)) RandomName();
    }

    public void RandomName()
    {
        _playerName.Value = new FixedString128Bytes(RandomString(7));
    }
    
    private static Random random = new Random();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}