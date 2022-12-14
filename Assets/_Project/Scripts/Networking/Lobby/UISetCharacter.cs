//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class UISetCharacter : MonoBehaviour
{
    [SerializeField] private UnityEvent onNewCharacterSelected;
    
    public void NextCharacter()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("You can't change characters on the server.");
            return;
        }

        onNewCharacterSelected.Invoke();
        PlayerData.LocalPlayerData.SetCharacterId(CharacterManager.Instance.GetNextId(PlayerData.LocalPlayerData.CharacterId));
    }

    public void PreviousCharacter()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("You can't change characters on the server.");
            return;
        }

        onNewCharacterSelected.Invoke();
        PlayerData.LocalPlayerData.SetCharacterId(CharacterManager.Instance.GetPreviousId(PlayerData.LocalPlayerData.CharacterId));
    }
}