//(c) copyright by Martin M. Klöckener
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Chat : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI chatLog;
    [SerializeField] private InputHelper chatInputField;
    
    private NetworkList<ChatMessage> _chatMessages;
    private readonly Dictionary<Guid/*clientGuid*/, string/*playerName*/> _playerNamesHistory = new();
    private string _chatMessagesFormatted = string.Empty;

    private void Awake()
    {
        _chatMessages = new NetworkList<ChatMessage>();
    }

    private void Initialize()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Could not find network manager! Chat will not be available.");
            return;
        }
        
        //subscribe to server callback
        _chatMessages.OnListChanged += OnChatMessagesChanged;
        
        //subscribe to user input
        chatInputField.OnUserEnteredMessage += CreateChatMessage;
        
        //show messages if there are any
        DisplayChatLog();
    }
    
    public override void OnDestroy()
    {
        //unsubscribe from server callback
        _chatMessages.OnListChanged -= OnChatMessagesChanged;
        
        //unsubscribe from user input
        chatInputField.OnUserEnteredMessage -= CreateChatMessage;
        
        //dispose NetworkVariables (if we don't do this, there will be memory leaks)
        _chatMessages.Dispose();
    }

    private void Start() => Initialize();
    private void OnChatMessagesChanged(NetworkListEvent<ChatMessage> changeEvent) => DisplayChatLog();

    private void DisplayChatLog()
    {
        FormatChatMessages();
        PrintChatMessages();
    }

    private void Update()
    {
        if (!IsServer) return;
        UpdatePlayerNames();
    }

    private void CreateChatMessage(string message)
    {
        //get local player ID and player name
        var playerData = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerData>();
        string playerName = playerData.PlayerName;
        Guid clientGuid = playerData.ClientGuid;
        
        //create chatMessageStruct
        var chatMessage = new ChatMessage(clientGuid, playerName, message);

        //add to syncList in order to let other clients know about the message
        SendChatMessageServerRpc(chatMessage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(ChatMessage message)
    {
        UpdateNamesInMessageLog();
        _chatMessages.Add(message);
    }

    private void FormatChatMessages()
    {
        _chatMessagesFormatted = "Welcome in chat!";

        if (_chatMessages.Count > 0) _chatMessagesFormatted += "\n\n";
        
        foreach (ChatMessage message in _chatMessages)
        {
            _chatMessagesFormatted += $"{message.PlayerName.Value}: {message.Message.Value}\n";
        }
    }

    private void PrintChatMessages()
    {
        chatLog.SetText(_chatMessagesFormatted);
    }

    private void UpdatePlayerNames()
    {
        foreach (KeyValuePair<ulong, NetworkClient> connectedClient in NetworkManager.ConnectedClients)
        {
            
            var playerData = connectedClient.Value.PlayerObject.GetComponent<PlayerData>();
            Guid clientGuid = playerData.ClientGuid;
            bool playerNamesDirty = false;
            
            Debug.Log($"clientGuid: {clientGuid.ToString()}");
            
            //update names of connected players and still keep old names of disconnected players
            if (_playerNamesHistory.ContainsKey(clientGuid))
            {
                //if the player name has changed...
                if (_playerNamesHistory[clientGuid] != playerData.PlayerName)
                {
                    //... update it ...
                    _playerNamesHistory[clientGuid] = playerData.PlayerName;
                    playerNamesDirty = true;
                }
            }

            //if the player name is not yet registered, add it to the dictionary
            else
            {
                _playerNamesHistory.Add(clientGuid, playerData.PlayerName);
                playerNamesDirty = true;
            }
            
            //if the player names have changed, update the chat messages
            if(playerNamesDirty) UpdateNamesInMessageLog();
        }
    }

    private void UpdateNamesInMessageLog()
    {
        for (int i = _chatMessages.Count - 1; i >= 0; i--)
        {
            if (_playerNamesHistory.ContainsKey(_chatMessages[i].ClientGuid))
            {
                ChatMessage message = _chatMessages[i];
                message.PlayerName = new FixedString128Bytes(_playerNamesHistory[message.ClientGuid]);
                _chatMessages[i] = message;
            }
            else
            {
                ChatMessage message = _chatMessages[i];
                message.PlayerName = new FixedString128Bytes("Unknown Player");
                _chatMessages[i] = message;
            }
        }
    }
}

public struct ChatMessage : INetworkSerializable, IEquatable<ChatMessage>
{
    public Guid ClientGuid => Guid.Parse(_clientGuid.Value);
    
    private FixedString64Bytes _clientGuid;
    public FixedString128Bytes PlayerName;
    public FixedString512Bytes Message;

    public ChatMessage(Guid clientGuid, FixedString128Bytes playerName, FixedString512Bytes message)
    {
        _clientGuid = new FixedString64Bytes(clientGuid.ToString());
        PlayerName = playerName;
        Message = message;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _clientGuid);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Message);
    }

    public bool Equals(ChatMessage other) => _clientGuid.Equals(other._clientGuid) && PlayerName.Equals(other.PlayerName) && Message.Equals(other.Message);
    public override bool Equals(object obj) => obj is ChatMessage other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_clientGuid, PlayerName, Message);
}