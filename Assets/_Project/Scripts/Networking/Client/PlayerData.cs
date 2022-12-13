//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Wichtel;
using Wichtel.Extensions;

public class PlayerData : NetworkBehaviour
{
    public static PlayerData LocalPlayerData => (localPlayerDataCached == null || localPlayerDataCached.IsDestroyed()) ? CacheLocalPlayerData() : localPlayerDataCached;
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
            return _playerNameIsSynced ? _playerName.Value.ToString() : _playerNameLocal;
        }
    }

    public uint CharacterId
    {
        get
        {
            if (!IsLocalPlayer) return _characterId.Value;
            return _characterIdIsSynced ? _characterId.Value : _characterIdLocal;
        }
    }

    public string Prompt => _prompt.Value.ToString();
    public int PointsCreativity => _pointsCreativity.Value;
    public int PointsPlayability => _pointsPlayability.Value;
    public int PointsPerformance => _pointsPerformance.Value;

    //we need these pre-loaded values because even if the player name was loaded from player prefs, it's only set in the first sync from the server. With these values we can access the correct name and characterId right from the beginning
    private string _playerNameLocal = string.Empty;
    private uint _characterIdLocal = 0;

    private bool _playerNameIsSynced = false;
    private bool _characterIdIsSynced = false;

    private string _defaultPlayerName;

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
        //default playerName
        _defaultPlayerName = $"Player {OwnerClientId.ToString()}";
        
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
                _playerNameLocal = _defaultPlayerName;
                SetPlayerNameServerRpc(_defaultPlayerName);
            }
            //if the name is longer than 18 characters, it was not the real previous name but was changed in the registry. Set it to something default to avoid bugs.
            else if (previousName.Length > 18) previousName = "Little Hacker";

            _playerNameLocal = previousName;
            SetPlayerNameServerRpc(previousName);
        }
        else
        {
            //if there is no previous name in player prefs, create default name ("Player 1")
            _playerNameLocal = _defaultPlayerName;
            SetPlayerNameServerRpc(_defaultPlayerName);
        }

        if (PlayerPrefs.HasKey("characterId"))
        {
            //load previous characterId
            int previousCharacterId = PlayerPrefs.GetInt("characterId");
            //set the id
            _characterIdLocal = (uint)previousCharacterId;
            SetCharacterIdServerRpc((uint)previousCharacterId);
        }
        else
        {
            _characterIdLocal = 0;
            _characterId = new NetworkVariable<uint>(0);
        }
        
        //subscribe to change events
        _playerName.OnValueChanged += OnPlayerNameChanged;
        _characterId.OnValueChanged += OnCharacterIdChanged;
    }

    public override void OnDestroy()
    {
        //reset local data
        if (IsLocalPlayer) localPlayerDataCached = null;
        
        //unsubscribe form events
        _playerName.OnValueChanged -= OnPlayerNameChanged;
        _characterId.OnValueChanged -= OnCharacterIdChanged;
        
        //dispose NetworkVariables (if we don't do this, there will be memory leaks)
        _clientGuid.Dispose();
        _playerName.Dispose();
        _characterId.Dispose();
        _prompt.Dispose();
        _pointsCreativity.Dispose();
        _pointsPlayability.Dispose();
        _pointsPerformance.Dispose();
    }

    //Change events
    private void OnPlayerNameChanged(FixedString128Bytes previousvalue, FixedString128Bytes newvalue)
    {
        //save the name to the player prefs (we do it here instead of in SetPlayerName() to avoid names with profanity being saved to the prefs)
        if (IsLocalPlayer)
        {
            //if the new name is the default player name, remove the entry from player prefs. This prevents old names to be retrieved if the player chose to play with the default name.
            if (newvalue == _defaultPlayerName)
                PlayerPrefs.DeleteKey("playerName");
            //otherwise, save it to the prefs
            else
                PlayerPrefs.SetString("playerName", newvalue.Value);
        }

        Debug.Log("OnPlayerNameChanged");
        
        _playerNameIsSynced = true;
    }

    private void OnCharacterIdChanged(uint previousvalue, uint newvalue)
    {
        //if the local characterId matches the one that came from the server, it's synced
        //ja, dadurch geben wir dem Client die volle Kontrolle --> cheatinganfällig. Bei der Charakterauswahl ist das aber nicht tragisch.
        if(newvalue == _characterIdLocal) _characterIdIsSynced = true;
    }

    //Setters
    [ServerRpc]
    private void SetClientGuidServerRpc(FixedString64Bytes newGuid)
    {
        _clientGuid.Value = newGuid;
    }
    
    //Call this on the client!
    public void SetPlayerName(string newName)
    {
        _playerNameIsSynced = false;
        _playerNameLocal = string.IsNullOrWhiteSpace(newName) ? _defaultPlayerName : newName;
        SetPlayerNameServerRpc(newName);
    }

    //here we check the name for profanity and then set it (default name if profanity was found)
    [ServerRpc]
    private void SetPlayerNameServerRpc(string newName)
    {
        //set the name to something different so that the onChanged event always gets called, even if the name didn't change (we need this in order to update the view correctly on clients)
        _playerName.Value = "DEFAULT_TEXT_SO_THAT_CHANGED_EVENT_GETS_CALLED";
        
        //if the new value is empty or whitespace, use the default name instead
        if (string.IsNullOrWhiteSpace(newName))
            _playerName.Value = _defaultPlayerName;
        //then check if the name contains profanity. if it does, use the default name. if it doesn't use the new name
        else
            _playerName.Value = ProfanityFilter.Instance.ContainsProfanity(newName) ? _defaultPlayerName :  new FixedString128Bytes(newName);
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
        
        //set the local characterID (to avoid a laggy feel when selecting the character)
        _characterIdLocal = newCharacterId;
        _characterIdIsSynced = false;
        
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
    public void SetPromptServerRpc(string newPrompt)
    {
        _prompt.Value = new FixedString512Bytes(newPrompt);
    }
    
    //The points should not have a ServerRPC (the client should not have the authority to set its points). The server should calculate those based on the user input
    public void SetPointsCreativity(int newValue) => _pointsCreativity.Value = newValue;
    public void SetPointsPlayability(int newValue) => _pointsPlayability.Value = newValue;
    public void SetPointsPerformance(int newValue) => _pointsPerformance.Value = newValue;
}