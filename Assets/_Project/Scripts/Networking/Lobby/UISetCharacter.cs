//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UISetCharacter : MonoBehaviour
{
    public void NextCharacter()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("You can't change characters on the server.");
            return;
        }
        
        PlayerData.LocalPlayerData.SetCharacterId(CharacterManager.Instance.GetNextId(PlayerData.LocalPlayerData.CharacterId));
    }

    public void PreviousCharacter()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("You can't change characters on the server.");
            return;
        }
        
        PlayerData.LocalPlayerData.SetCharacterId(CharacterManager.Instance.GetPreviousId(PlayerData.LocalPlayerData.CharacterId));
    }
}