//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    [DisplayAsString] public string PlayerName => _playerName.ToString();

    //Network Variables
    private NetworkVariable<FixedString32Bytes> _playerName = new("Unknown Player");
}