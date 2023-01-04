using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Wichtel.Extensions;

public class AssignPrompts : MonoBehaviour
{
    private void Start()
    {
        //only run on server
        if (!NetworkManager.Singleton.IsServer) return;

        var prompts = new List<string>();

        //get prompts
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
        {
            prompts.Add(NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerData>().Prompt);
        }

        //shuffle prompts
        prompts.ShuffleGuaranteedIndexChange();

        //assign shuffled prompts
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
        {
            NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerData>().SetAssignedPrompt(prompts[i]);
        }
    }
}
