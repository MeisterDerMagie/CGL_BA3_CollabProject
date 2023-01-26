//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AssignBackingTrack : MonoBehaviour
{
    private void Start()
    {
        //only run on server
        if (!NetworkManager.Singleton.IsServer) return;

        //get random Id
        int backingTrackId = BackingTrackManager.Instance.GetRandomBackingTrackId();

        //set it to every player data
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerData>().SetBackingTrackId(backingTrackId);
        }
    }
}