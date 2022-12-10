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
    private readonly Dictionary<ulong/*clientId*/, string/*playerName*/> _playerNamesHistory = new();
    private string _chatMessagesFormatted = string.Empty;

    
    private void Initialize()
    {
        _chatMessages = new NetworkList<ChatMessage>();
        
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
        //DEBUG
        Debug.Log("You entered a chat message.");
        
        //get local player ID and player name
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        string playerName = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerData>().PlayerName;
        
        //create chatMessageStruct
        var chatMessage = new ChatMessage(localClientId, playerName, message);

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
            _chatMessagesFormatted += $"{message.PlayerName.ToString()}: {message.Message.ToString()}\n";
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
            bool playerNamesDirty = false;
            
            //update names of connected players and still keep old names of disconnected players
            if (_playerNamesHistory.ContainsKey(connectedClient.Key))
            {
                //if the player name has changed...
                if (_playerNamesHistory[connectedClient.Key] != playerData.PlayerName)
                {
                    //... update it ...
                    _playerNamesHistory[connectedClient.Key] = playerData.PlayerName;
                    playerNamesDirty = true;
                }
            }

            //if the player name is not yet registered, add it to the dictionary
            else
            {
                _playerNamesHistory.Add(connectedClient.Key, playerData.PlayerName);
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
            if (_playerNamesHistory.ContainsKey(_chatMessages[i].ClientId))
            {
                ChatMessage message = _chatMessages[i];
                message.PlayerName = _playerNamesHistory[message.ClientId];
                _chatMessages[i] = message;
            }
            else
            {
                ChatMessage message = _chatMessages[i];
                message.PlayerName = "Unkown Player";
                _chatMessages[i] = message;
            }
        }
    }
}

public struct ChatMessage : INetworkSerializable, IEquatable<ChatMessage>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public FixedString512Bytes Message;

    public ChatMessage(ulong clientId, FixedString32Bytes playerName, FixedString512Bytes message)
    {
        PlayerName = playerName;
        Message = message;
        ClientId = clientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Message);
    }

    public bool Equals(ChatMessage other) => ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && Message.Equals(other.Message);
    public override bool Equals(object obj) => obj is ChatMessage other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(ClientId, PlayerName, Message);
}