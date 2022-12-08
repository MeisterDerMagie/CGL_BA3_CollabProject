using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> testInt = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<string> testString = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private CoroutineHandle coroutine;
    
    #if !UNITY_SERVER
    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn: {OwnerClientId}");

        testString.OnValueChanged += OnTestStringChanged;
        if(IsLocalPlayer) coroutine = Timing.RunCoroutine(_NewInt());
    }

    private void OnTestStringChanged(string previousvalue, string newvalue)
    {
        Debug.Log($"Player {OwnerClientId.ToString()} said: {newvalue}");
    }

    private IEnumerator<float> _NewInt()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(2);
            testInt.Value = Random.Range(0, 100);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (coroutine.IsRunning) Timing.KillCoroutines(coroutine);
    }

    [Button]
    private void SaySomething(string text) => testString.Value = text;
    #endif
}
