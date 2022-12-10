//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

public class PlayerData : NetworkBehaviour
{
    public Guid ClientGuid => Guid.Parse(_clientGuid.Value.ToString());
    public string PlayerName => _playerName.Value.ToString();
    public uint CharacterId => _characterId.Value;
    public string Prompt => _prompt.Value.ToString();
    public int PointsCreativity => _pointsCreativity.Value;
    public int PointsPlayability => _pointsPlayability.Value;
    public int PointsPerformance => _pointsPerformance.Value;

    //Network Variables
    private NetworkVariable<FixedString64Bytes> _clientGuid;
    private NetworkVariable<FixedString128Bytes> _playerName;
    private NetworkVariable<uint> _characterId;
    private NetworkVariable<FixedString512Bytes> _prompt;

    private NetworkVariable<int> _pointsCreativity;
    private NetworkVariable<int> _pointsPlayability;
    private NetworkVariable<int> _pointsPerformance;

    //Initialization
    private void Awake()
    {
        //instantiate NetworkVariables
        _clientGuid = new NetworkVariable<FixedString64Bytes>(Guid.NewGuid().ToString());
        _playerName = new NetworkVariable<FixedString128Bytes>(string.Empty);
        _characterId = new NetworkVariable<uint>(0);
        _prompt = new NetworkVariable<FixedString512Bytes>(string.Empty);
        _pointsCreativity = new NetworkVariable<int>(0);
        _pointsPlayability = new NetworkVariable<int>(0);
        _pointsPerformance = new NetworkVariable<int>(0);
    }

    private void Start()
    {
        //load player prefs
        if (!IsLocalPlayer) return;
        
        //load client guid
        if (PlayerPrefs.HasKey("clientGuid"))
        {
            string clientGuidAsString = PlayerPrefs.GetString("clientGuid");
            Guid clientGuid = Guid.Parse(clientGuidAsString);
            SetClientGuidServerRpc(clientGuid.ToString());
        }
        else
        {
            Guid newGuid = Guid.NewGuid();
            SetClientGuidServerRpc(newGuid.ToString());
            PlayerPrefs.SetString("clientGuid", newGuid.ToString());
        }
        
        //load last name and characterId from player prefs ... or create default if it's the first time playing
        if (PlayerPrefs.HasKey("playerName"))
        {
            //load previous name
            string previousName = PlayerPrefs.GetString("playerName");
            if(previousName == null) SetPlayerNameServerRpc($"Player {OwnerClientId.ToString()}");
            //if the name is longer than 20 characters, it was not the real previous name but was changed in the registry. Set it to something default to avoid bugs.
            if (previousName.Length > 20) previousName = "Little Hacker";
            SetPlayerNameServerRpc(previousName);
        }
        else
        {
            SetPlayerNameServerRpc($"Player {OwnerClientId.ToString()}");
            //_playerName = new NetworkVariable<FixedString128Bytes>(new FixedString128Bytes($"Player {OwnerClientId.ToString()}"));
        }

        if (PlayerPrefs.HasKey("characterId"))
        {
            //load previous characterId
            int previousCharacterId = PlayerPrefs.GetInt("characterId");
            //set the id
            SetCharacterIdServerRpc((uint)previousCharacterId);
        }
        else
        {
            _characterId = new NetworkVariable<uint>(0);
        }
    }

    public override void OnDestroy()
    {
        //dispose NetworkVariables (if we don't do this, there will be memory leaks)
        _clientGuid.Dispose();
        _playerName.Dispose();
        _characterId.Dispose();
        _prompt.Dispose();
        _pointsCreativity.Dispose();
        _pointsPlayability.Dispose();
        _pointsPerformance.Dispose();
    }

    //Setters
    [ServerRpc]
    private void SetClientGuidServerRpc(FixedString64Bytes newGuid)
    {
        _clientGuid.Value = newGuid;
    }
    
    //Call this on the client! Here we set the player name and save it in the player prefs. 
    public void SetPlayerName(string newName)
    {
        PlayerPrefs.SetString("playerName", newName);
        SetPlayerNameServerRpc(newName);
    }
    
    //here we actually set the name, but it doesn't get saved to the player prefs. This is needed for initializing the playerName. We don't want the game to save "Player 1" to the prefs.
    [ServerRpc]
    private void SetPlayerNameServerRpc(string newName)
    {
        _playerName.Value = new FixedString128Bytes(newName);
    }

    //Call this on the client! here we check if the characterId is valid (aka there exists a corresponding character), then save it to the playerPrefs and call the ServerRPC
    public void SetCharacterId(uint newCharacterId)
    {
        //check if id is valid
        if (!CharacterManager.Instance.IsValidCharacterId(newCharacterId))
        {
            Debug.LogWarning("You managed to choose a non-existent character. Congratulations!");
            return;
        }
        
        //load player pref
        PlayerPrefs.SetInt("characterId", (int)newCharacterId);
        
        //call serverRPC to actually set the variable
        SetCharacterIdServerRpc(newCharacterId);
    }
    
    [ServerRpc]
    private void SetCharacterIdServerRpc(uint newCharacterId)
    {
        //check if id is valid (yes we need to do this in ServerRPC and the non-rpc method in order to prevent invalid ids by users hacking in the registry)
        if (!CharacterManager.Instance.IsValidCharacterId(newCharacterId))
        {
            Debug.LogWarning("You managed to choose a non-existent character. Congratulations!");
            return;
        }
        
        _characterId.Value = newCharacterId;
    }

    [ServerRpc]
    public void SetPromptServerRpc(string newPrompt)
    {
        _prompt.Value = new FixedString512Bytes(newPrompt);
    }
    
    //The points should not have a ServerRPC (the client should not have the authority to set its points). The server should calculate those based on the user input
    public void SetPointsCreativity(int newValue) => _pointsCreativity.Value = newValue;
    public void SetPointsPlayability(int newValue) => _pointsPlayability.Value = newValue;
    public void SetPointsPerformance(int newValue) => _pointsPerformance.Value = newValue;
}