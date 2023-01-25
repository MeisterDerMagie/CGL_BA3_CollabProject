//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayabilityCalculation : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayabilityServerRpc(string performingPlayerGuid, string ratedPlayerGuid, float percent)
    {
        
    }
}