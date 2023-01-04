//(c) copyright by Martin M. Klöckener
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Wichtel;

public class Chat : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI chatLog;
    [SerializeField] private InputHelper chatInputField;

    public UnityEvent<int> onNewMessages; //int is the amount of new messages
    public UnityEvent<ChatMessage> onNewMessage; //string is the new message

    private NetworkList<ChatMessage> _chatMessages;
    private List<ChatMessage> _chatMessagesLocal;
    private readonly Dictionary<Guid/*clientGuid*/, string/*playerName*/> _playerNamesHistory = new();
    private string _chatMessagesFormatted = string.Empty;

    private void Awake()
    {
        _chatMessages = new NetworkList<ChatMessage>();
        _chatMessagesLocal = new List<ChatMessage>();
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
        if(NetworkManager.IsServer)
            DisplayChatLog();
        else
            SyncAndDisplay();
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
    private void OnChatMessagesChanged(NetworkListEvent<ChatMessage> changeEvent) => SyncAndDisplay();

    private void SyncAndDisplay()
    {
        //--- events ---
        //get new messages and call OnNewMessage event if there are any
        //NetworkList doesn't have .Except(), so we need to create a new list of it
        var chatMessagesFromServer = new List<ChatMessage>();
        foreach (var chatMessage in _chatMessages)
        {
            chatMessagesFromServer.Add(chatMessage);
        }

        //extract all new messages
        var newMessages = chatMessagesFromServer.Except(_chatMessagesLocal).ToList();

        //remove own messages
        if (!NetworkManager.IsServer)
        {
            Guid ownGuid = PlayerData.LocalPlayerData.ClientGuid;
            for (int i = newMessages.Count - 1; i >= 0; i--)
            {
                if(newMessages[i].ClientGuid == ownGuid) newMessages.RemoveAt(i);
            }   
        }

        //call event that there genereally have been new messages (e.g. for a notification sound)
        if (newMessages.Count > 0) onNewMessages.Invoke(newMessages.Count);

        //call events for each new individual message
        foreach (ChatMessage chatMessage in newMessages)
        {
            onNewMessage.Invoke(chatMessage);
        }
        //-- end of events --
        
        //sync localMessages to the server-validated messages
        _chatMessagesLocal.Clear();
        foreach (ChatMessage chatMessage in _chatMessages)
        {
            _chatMessagesLocal.Add(chatMessage);
        }
        
        //display the log
        DisplayChatLog();
    }
    
    private void DisplayChatLog()
    {
        //format messages and display them
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
        if (IsServer)
        {
            Debug.LogWarning("You can't send chat messages from from the server.");
            return;
        }

        //get local player ID and player name
        var playerData = PlayerData.LocalPlayerData;
        string playerName = playerData.PlayerName;
        Guid clientGuid = playerData.ClientGuid;
        
        //create chatMessageStruct
        var chatMessage = new ChatMessage(clientGuid, playerName, message, DateTime.Now.Ticks);

        //add to local message to show it locally before it was validated by the server (feels less laggy)
        _chatMessagesLocal.Add(chatMessage);
        DisplayChatLog();

        //add to syncList in order to let other clients know about the message
        SendChatMessageServerRpc(chatMessage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(ChatMessage message)
    {
        UpdateNamesInMessageLog();
        
        //censor message
        message.Message = ProfanityFilter.Instance.CensorAndReplaceWholeInput(message.Message.Value);

        //then send it to all clients
        _chatMessages.Add(message);
    }

    private void FormatChatMessages()
    {
        _chatMessagesFormatted = "Welcome in chat!";

        if (_chatMessagesLocal.Count > 0) _chatMessagesFormatted += "\n\n";
        
        foreach (ChatMessage message in _chatMessagesLocal)
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
    private long _timestamp; //we need this for the "new message" event to work. otherwise two same messages are not considered a new message

    public ChatMessage(Guid clientGuid, FixedString128Bytes playerName, FixedString512Bytes message, long timestamp)
    {
        _clientGuid = new FixedString64Bytes(clientGuid.ToString());
        PlayerName = playerName;
        Message = message;
        _timestamp = timestamp;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _clientGuid);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Message);
        serializer.SerializeValue(ref _timestamp);
    }

    public bool Equals(ChatMessage other) => _clientGuid.Equals(other._clientGuid) && PlayerName.Equals(other.PlayerName) && Message.Equals(other.Message) && _timestamp == other._timestamp;
    public override bool Equals(object obj) => obj is ChatMessage other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_clientGuid, PlayerName, Message, _timestamp);
}