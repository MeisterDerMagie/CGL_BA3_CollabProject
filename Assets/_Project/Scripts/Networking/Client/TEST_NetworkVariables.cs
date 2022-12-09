using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class TEST_NetworkVariables : NetworkBehaviour
{
    
    [SerializeField] private NetworkVariable<int> testInt = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<FixedString64Bytes> testString = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private CoroutineHandle coroutine;
    
    #if !UNITY_SERVER
    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn: {OwnerClientId.ToString()}");

        testString.OnValueChanged += OnTestStringChanged;
        if(IsLocalPlayer) coroutine = Timing.RunCoroutine(_NewInt());
    }

    private void OnTestStringChanged(FixedString64Bytes previousvalue, FixedString64Bytes newvalue)
    {
        Debug.Log($"Player {OwnerClientId.ToString()} said: {newvalue.ToString()}");
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
    #endif
}
