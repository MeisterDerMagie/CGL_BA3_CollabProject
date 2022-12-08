using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> testInt = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    #if !UNITY_SERVER
    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn");
        
        Timing.RunCoroutine(_NewInt());
    }

    private IEnumerator<float> _NewInt()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(2);
            testInt.Value = Random.Range(0, 100);
        }
    }
    #endif
}
