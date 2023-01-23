using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class HitFeedback : NetworkBehaviour
{
    [SerializeField] private List<HitFeedbackIcon> feedbackIcons = new();

    private NetworkList<uint> _characterIds;
    private Dictionary<ulong, uint> _clientIdCharacterIdMap = new();

    private void Awake()
    {
        _characterIds = new NetworkList<uint>();
    }

    public override void OnNetworkSpawn()
    {
        //Server
        if (NetworkManager.Singleton.IsServer)
        {
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.ConnectedClients)
            {
                ulong clientId = client.Key;
                uint characterId = client.Value.PlayerObject.GetComponent<PlayerData>().CharacterId;
                
                _clientIdCharacterIdMap.Add(clientId, characterId);
                _characterIds.Add(characterId);
            }
        }
        
        //Client
        else
        {
            UpdateCharacterIcons();
            _characterIds.OnListChanged += UpdateCharacterIcons;
        }
    }


    private void UpdateCharacterIcons(NetworkListEvent<uint> changeevent) => UpdateCharacterIcons();
    private void UpdateCharacterIcons()
    {
        foreach (HitFeedbackIcon feedbackIcon in feedbackIcons)
        {
            int index = feedbackIcons.IndexOf(feedbackIcon);
            uint characterId = uint.MaxValue;
            
            if (_characterIds.Count > index)
            {
                characterId = _characterIds[index];
            }

            feedbackIcon.SetPlayerIcon(characterId);
        }
    }

    /// <summary> Call this on the client whenever they score. </summary>
    [ContextMenu("SendHitFeedback")]
    public void SendHitFeedback(BeatMapping.ScoringType scoringType)
    {
        SendHitFeedbackServerRpc(NetworkManager.LocalClientId, scoringType);
    }

    [ServerRpc]
    private void SendHitFeedbackServerRpc(ulong clientId, BeatMapping.ScoringType scoringType)
    {
        BroadcastFeedbackClientRpc(_clientIdCharacterIdMap[clientId], scoringType);
    }

    [ClientRpc]
    private void BroadcastFeedbackClientRpc(uint characterId, BeatMapping.ScoringType scoringType)
    {
        if (!_characterIds.Contains(characterId))
        {
            Debug.LogError("Can't play Hit Feedback because no matching characterId was found.", this);
            return;
        }

        int index = _characterIds.IndexOf(characterId);
        feedbackIcons[index].ShowFeedback(scoringType);
    }

    public override void OnDestroy()
    {
        _characterIds.Dispose();
    }
}
