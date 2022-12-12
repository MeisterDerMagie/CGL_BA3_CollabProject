//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public static PlayerData LocalPlayerData => (localPlayerDataCached == null) ? CacheLocalPlayerData() : localPlayerDataCached;
    private static PlayerData localPlayerDataCached;

    private static PlayerData CacheLocalPlayerData()
    {
        localPlayerDataCached = (NetworkManager.Singleton == null || NetworkManager.Singleton.LocalClient == null || NetworkManager.Singleton.LocalClient.PlayerObject == null) ? null : NetworkManager.Singleton.LocalClient?.PlayerObject.GetComponent<PlayerData>();
        return localPlayerDataCached;
    }
    
    public Guid ClientGuid => Guid.Parse(_clientGuid.Value.ToString());
    public string PlayerName
    {
        get
        {
            if (!IsLocalPlayer) return _playerName.Value.ToString();
            return _playerNameIsSynced ? _playerName.Value.ToString() : _loadedPlayerName;
        }
    }

    public uint CharacterId
    {
        get
        {
            if (!IsLocalPlayer) return _characterId.Value;
            return _characterIdIsSynced ? _characterId.Value : _loadedCharacterId;
        }
    }

    public bool IsReadyInLobby => _isReadyInLobby.Value;
    public string Prompt => _prompt.Value.ToString();
    public int PointsCreativity => _pointsCreativity.Value;
    public int PointsPlayability => _pointsPlayability.Value;
    public int PointsPerformance => _pointsPerformance.Value;

    //we need these pre-loaded values because even if the player name was loaded from player prefs, it's only set in the first sync from the server. With these values we can access the correct name and characterId right from the beginning
    private string _loadedPlayerName = string.Empty;
    private uint _loadedCharacterId = 0;

    private bool _playerNameIsSynced = false;
    private bool _characterIdIsSynced = false;
    
    //Network Variables
    private NetworkVariable<FixedString64Bytes> _clientGuid;
    private NetworkVariable<FixedString128Bytes> _playerName;
    private NetworkVariable<uint> _characterId;
    private NetworkVariable<bool> _isReadyInLobby;
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
        _isReadyInLobby = new NetworkVariable<bool>(false);
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
            if (previousName == null)
            {
                string genericPlayerName = $"Player {OwnerClientId.ToString()}";
                _loadedPlayerName = genericPlayerName;
                SetPlayerNameServerRpc(genericPlayerName);
            }
            //if the name is longer than 18 characters, it was not the real previous name but was changed in the registry. Set it to something default to avoid bugs.
            else if (previousName.Length > 18) previousName = "Little Hacker";

            _loadedPlayerName = previousName;
            SetPlayerNameServerRpc(previousName);
        }
        else
        {
            string genericPlayerName = $"Player {OwnerClientId.ToString()}";
            _loadedPlayerName = genericPlayerName;
            SetPlayerNameServerRpc(genericPlayerName);
        }

        if (PlayerPrefs.HasKey("characterId"))
        {
            //load previous characterId
            int previousCharacterId = PlayerPrefs.GetInt("characterId");
            //set the id
            _loadedCharacterId = (uint)previousCharacterId;
            SetCharacterIdServerRpc((uint)previousCharacterId);
        }
        else
        {
            _loadedCharacterId = 0;
            _characterId = new NetworkVariable<uint>(0);
        }
        
        //subscribe to change events
        _playerName.OnValueChanged += OnPlayerNameChanged;
        _characterId.OnValueChanged += OnCharacterIdChanged;
    }

    public override void OnDestroy()
    {
        //unsubscribe form events
        _playerName.OnValueChanged -= OnPlayerNameChanged;
        _characterId.OnValueChanged -= OnCharacterIdChanged;
        
        //dispose NetworkVariables (if we don't do this, there will be memory leaks)
        _clientGuid.Dispose();
        _playerName.Dispose();
        _characterId.Dispose();
        _isReadyInLobby.Dispose();
        _prompt.Dispose();
        _pointsCreativity.Dispose();
        _pointsPlayability.Dispose();
        _pointsPerformance.Dispose();
    }

    //Change events
    private void OnPlayerNameChanged(FixedString128Bytes previousvalue, FixedString128Bytes newvalue) => _playerNameIsSynced = true;
    private void OnCharacterIdChanged(uint previousvalue, uint newvalue) => _characterIdIsSynced = true;

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
        
        //set player pref
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
    public void SetReadyStateServerRpc(bool newState)
    {
        _isReadyInLobby.Value = newState;
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