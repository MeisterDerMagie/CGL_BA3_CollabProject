//(c) copyright by Martin M. Klöckener
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Wichtel.GitVersion;

public class ServerProviderCommunication : MonoBehaviour
{
    [Title("Change IP and Port in serverConfig.json in StreamingAssets folder of built game.")]
    [SerializeField, ReadOnly] private string serverIp = "127.0.0.1";
    [SerializeField, ReadOnly] private int serverPort = 50880;
    
    private static ServerProviderCommunication instance;
    public static ServerProviderCommunication Instance => instance;
    
    private UnityTransport _transport;
    private UnityTransport Transport
    {
        get
        {
            if (_transport != null) return _transport;
            _transport = FindObjectOfType<UnityTransport>();
            if(_transport == null) Debug.LogError("transport is null!");
            return _transport;
        }
    }

    private ServerProviderClient _client;
    private bool _disconnectClientOnDestroy = true;

    private void Awake()
    {
       LoadServerConfig();

       if(instance == null) instance = this;
        else
        {
            _disconnectClientOnDestroy = false;
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        
        //start client
        _client = new ServerProviderClient(serverIp, serverPort);
        
        //subscribe to onServerStarted event
        NetworkManager.Singleton.OnServerStarted += ServerStarted;
    }

    private void OnDestroy()
    {
        if(this._disconnectClientOnDestroy)
            ServerProviderClient.DisconnectClient();

        if(NetworkManager.Singleton != null) NetworkManager.Singleton.OnServerStarted -= ServerStarted;
    }

    #region Outgoing
    //-- Client -> Provider --
    public void HostRequest()
    {
        var metadata = new Dictionary<object, object>();
        metadata.Add("version", GitVersion.Version.VersionString);
        ServerProviderClient.SendMessage("host", metadata);
    }

    public void CancelHostRequest()
    {
        ServerProviderClient.SendMessage("cancelHost", null);
    }

    public void JoinRequest(string lobbyCode)
    {
        var metadata = new Dictionary<object, object>();
        metadata.Add("lobbyCode", lobbyCode);
        metadata.Add("version", GitVersion.Version.VersionString);
        ServerProviderClient.SendMessage("join", metadata);
    }
    
    //-- Server -> Provider --
    public void ServerStarted()
    {
        var metadata = new Dictionary<object, object>();
        metadata.Add("lobbyCode", LobbyCode.Instance.code);
        metadata.Add("version", GitVersion.Version.VersionString);
        ServerProviderClient.SendMessage("serverStarted", metadata);
    }

    public void ServerInGame()
    {
        var metadata = new Dictionary<object, object>();
        metadata.Add("lobbyCode", LobbyCode.Instance.code);
        ServerProviderClient.SendMessage("serverInGame", metadata);
    }

    public void ServerIsFull()
    {
        var metadata = new Dictionary<object, object>();
        metadata.Add("lobbyCode", LobbyCode.Instance.code);
        ServerProviderClient.SendMessage("serverIsFull", metadata);
    }

    public void ServerIsNotFull()
    {
        var metadata = new Dictionary<object, object>();
        metadata.Add("lobbyCode", LobbyCode.Instance.code);
        ServerProviderClient.SendMessage("serverIsNotFull", metadata);
    }

    public void ServerStopped()
    {
        var metadata = new Dictionary<object, object>();
        metadata.Add("lobbyCode", LobbyCode.Instance.code);
        ServerProviderClient.SendMessage("serverStopped", metadata);
    }
    #endregion
    
    #region Incoming
    //-- Provider -> Client --
    public void ClientCanJoin(ushort port, string lobbyCode)
    {
        //set ip and port
        Transport.SetConnectionData(serverIp, port);
        
        //set lobby code
        LobbyCode.Instance.code = lobbyCode;
        
        //start client
        NetworkManager.Singleton.StartClient();
    }
    
    //-- Provider -> Server --

    #endregion

    private void LoadServerConfig()
    {
        //serverConfig.json should look like this: {"IP":"127.0.0.1","Port":"50880"}
        string encryptedJson = File.ReadAllText(Application.dataPath + "/StreamingAssets/serverConfig.json");
        
        Dictionary<string, string> content = JsonConvert.DeserializeObject<Dictionary<string, string>>(encryptedJson);
        if(content == null)
        {
            Debug.LogWarning("Could not load serverConfig. Will use localhost instead.");
            return;
        }

        if (content.ContainsKey("IP") && content.ContainsKey("Port"))
        {
            serverIp = content["IP"];
            serverPort = int.Parse(content["Port"]);

            Debug.Log($"<color=#25aaef>Loaded serverProvider IP:</color> {serverIp}");
            Debug.Log($"<color=#25aaef>Loaded serverProvider Port:</color> {serverPort.ToString()}");
        }
        else
        {
            Debug.LogWarning("<color=#25aaef>Could not load serverConfig. Will use localhost instead.</color>");
            return;
        }
    }
}