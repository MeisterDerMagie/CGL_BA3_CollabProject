//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayabilityCalculation : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayabilityServerRpc(FixedString128Bytes performingPlayerGuid, FixedString128Bytes ratedPlayerGuid, float percent)
    {
        
    }
}