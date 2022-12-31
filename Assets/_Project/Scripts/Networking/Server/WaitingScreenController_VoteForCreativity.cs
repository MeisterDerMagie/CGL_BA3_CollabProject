//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaitingScreenController_VoteForCreativity : WaitingScreenController
{
    //this gets only called on the client
    public override void Show()
    {
        transform.localScale = Vector3.one;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    //only call on the server
    public void ClientAwardedAllVotes(ulong clientId)
    {
        SetDoneState(clientId, true);
    }
}