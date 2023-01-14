using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class ScreenWaitingForOtherPlayers : NetworkBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private PlayerDoneIcon[] playerDoneIcons;
    private Dictionary<ulong, PlayerDoneIcon> _playerIcons;

    private void Awake()
    {
        //activate canvas
        canvas.gameObject.SetActive(true);
    }

    //only call this on the server
    public void Initialize(Dictionary<ulong, uint> players)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("This method should never be called on the client.");
            return;
        }

        Timing.RunCoroutine(_Initialize(players));
    }

    private IEnumerator<float> _Initialize(Dictionary<ulong, uint> players)
    {
        yield return Timing.WaitForSeconds(0.5f);
        
        //initially deactivate all icons (we activate all active ones in the next few lines)
        for (int i = 0; i < playerDoneIcons.Length; i++)
        {
            SetIconActiveClientRpc(i, false);
        }
        
        foreach (PlayerDoneIcon icon in playerDoneIcons)
        {
            icon.gameObject.SetActive(false);
        }
        
        //activate icons for connected players
        _playerIcons = new Dictionary<ulong, PlayerDoneIcon>();
        int index = 0;
        
        foreach (var player in players)
        {
            PlayerDoneIcon icon = playerDoneIcons[index];
            _playerIcons.Add(player.Key, icon);

            //set character
            icon.SetCharacter(player.Value);
            
            //activate icon game object
            SetIconActiveClientRpc(index, true);

            //increase index
            index += 1;
        }
    }

    [ClientRpc]
    private void SetIconActiveClientRpc(int iconIndex, bool value)
    {
        playerDoneIcons[iconIndex].gameObject.SetActive(value);
    }

    //only call this on the server
    public void SetDoneState(ulong clientId, bool isDone)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("This method should never be called on the client.");
            return;
        }
        
        foreach (var playerIcon in _playerIcons)
        {
            if(playerIcon.Key == clientId)
                playerIcon.Value.SetDoneState(isDone);
        }
    }
    
    #if UNITY_EDITOR
    private void Update()
    {
        PrefabStage currentPrefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
        
        //if we are not in prefab stage: hide the canvas because otherwise it will get into the way
        if(currentPrefabStage == null) canvas.gameObject.SetActive(false);

        //otherwise, if we are in prefab mode and editing this prefab: show the canvas (we want to edit it...)
        else
        {
            canvas.gameObject.SetActive(true);
        }
    }
    #endif
}
