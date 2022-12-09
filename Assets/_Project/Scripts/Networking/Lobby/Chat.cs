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
    
    /*
    private readonly SyncList<ChatMessage> chatMessages = new SyncList<ChatMessage>();
    private readonly NetworkList<ChatMessage>
    private string chatMessagesFormatted = string.Empty;
    */

    //private readonly SyncDictionary<uint /*netId*/, string /*playerName*/> playerNames = new SyncDictionary<uint, string>();
    //private Dictionary<uint /*netId*/, string /*playerName*/> playerNames = new Dictionary<uint, string>();

    /*
    private NetworkRoomManager manager;

    private void Initialize()
    {
        manager = FindObjectOfType<NetworkRoomManager>();
        if (manager == null)
        {
            Debug.LogError("Could not find network manager! Chat will not be available.");
            return;
        }
        
        //subscribe to server callback
        chatMessages.Callback += OnChatMessagesChanged;
        playerNames.Callback += OnPlayerNamesChanged;
        
        //subscribe to user input
        chatInputField.OnUserEnteredMessage += CreateChatMessage;
        
        //show messages if there are any
        DisplayChatLog();
    }

    private void Start() => Initialize();
    private void OnChatMessagesChanged(SyncList<ChatMessage>.Operation _op, int _itemindex, ChatMessage _olditem, ChatMessage _newitem) => DisplayChatLog();
    private void OnPlayerNamesChanged(SyncIDictionary<uint, string>.Operation _op, uint _key, string _item) => DisplayChatLog();


    private void DisplayChatLog()
    {
        FormatChatMessages();
        PrintChatMessages();
    }

    private void Update()
    {
        if (!isServer) return;
        UpdatePlayerNames();
    }

    private void CreateChatMessage(string _message)
    {
        //get local player
        NetworkRoomPlayer localPlayer = null;
        foreach (var roomPlayer in manager.roomSlots)
        {
            if (roomPlayer.isLocalPlayer) localPlayer = roomPlayer;
        }

        //get netId
        uint netId = localPlayer.netId;
        
        //create chatMessageStruct
        var chatMessage = new ChatMessage(netId, _message);

        //add to syncList in order to let other clients know about the message
        SendChatMessage(chatMessage);
    }

    [Command(requiresAuthority = false)]
    private void SendChatMessage(ChatMessage _message)
    {
        chatMessages.Add(_message);
    }

    private void FormatChatMessages()
    {
        chatMessagesFormatted = "Welcome in chat!";

        if (chatMessages.Count > 0) chatMessagesFormatted += "\n\n";
        
        foreach (ChatMessage message in chatMessages)
        {
            string playerName = playerNames.ContainsKey(message.netId) ? playerNames[message.netId] : "Unknown Player";
            chatMessagesFormatted += $"{playerName}: {message.message}\n";
        }
    }

    private void PrintChatMessages()
    {
        chatLog.SetText(chatMessagesFormatted);
    }

    [Server]
    private void UpdatePlayerNames()
    {
        foreach (NetworkRoomPlayer roomPlayer in manager.roomSlots)
        {
            string playerName = roomPlayer.GetComponent<RoomPlayerData>().PlayerName;
            
            //update names of connected players and still keep old names of disconnected players
            if (playerNames.ContainsKey(roomPlayer.netId))
            {
                //if the player name has changed...
                if (playerNames[roomPlayer.netId] != playerName)
                {
                    //... update it ... 
                    playerNames[roomPlayer.netId] = playerName;
                }
            }

            else
            {
                playerNames.Add(roomPlayer.netId, playerName);
            }
        }
    }

    private void OnDestroy()
    {
        //unsubscribe from user input
        chatInputField.OnUserEnteredMessage -= CreateChatMessage;
    }
}

public struct ChatMessage
{
    public uint netId;
    public FixedString512Bytes message;

    public ChatMessage(uint _netId, FixedString512Bytes _message)
    {
        netId = _netId;
        message = _message;
    }*/
}