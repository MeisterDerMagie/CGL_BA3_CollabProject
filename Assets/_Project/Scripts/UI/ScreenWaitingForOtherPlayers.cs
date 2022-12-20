using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;

public class ScreenWaitingForOtherPlayers : NetworkBehaviour
{
    [SerializeField] private PlayerDoneIcon playerDoneIconPrefab;
    [SerializeField] private NetworkObject iconsParent;
    
    private Dictionary<ulong, PlayerDoneIcon> _playerIcons;
    
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
        _playerIcons = new Dictionary<ulong, PlayerDoneIcon>();

        var spawnedNetworkObjects = new List<NetworkObject>();
        foreach (var player in players)
        {
            PlayerDoneIcon icon = Instantiate(playerDoneIconPrefab, iconsParent.transform, false);
            var iconNetworkObject = icon.GetComponent<NetworkObject>();
            iconNetworkObject.Spawn();

            //add to spawned collection in order to set the parent afterwards
            spawnedNetworkObjects.Add(iconNetworkObject);
            
            //set scale
            icon.transform.localScale = Vector3.one;

            //set character
            var playerDoneIcon = icon.GetComponent<PlayerDoneIcon>();
            playerDoneIcon.SetCharacter(player.Value);

            //add to collection
            _playerIcons.Add(player.Key, icon);
        }
        
        //set parent
        yield return Timing.WaitForSeconds(0.5f);

        for (int i = 0; i < spawnedNetworkObjects.Count; i++)
        {
            bool parentingSuccess = spawnedNetworkObjects[i].TrySetParent(iconsParent, false);

            if (!parentingSuccess)
            {
                Debug.LogError("Could not parent.", this);
                yield break;//return;
            }
        }
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
}
