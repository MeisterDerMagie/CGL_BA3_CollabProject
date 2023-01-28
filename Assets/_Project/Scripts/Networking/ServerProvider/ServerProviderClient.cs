using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;
using Wichtel.Threading;
using Debug = UnityEngine.Debug;

internal class ServerProviderClient
{
    public static bool Connected => _Client.Connected;
    
    private static string _ServerIp = "";
    private static int _ServerPort = 0;
    private static bool _DebugMessages = true;
    private static WatsonTcpClient _Client = null;
    
    public static Action OnCouldNotConnectToServerProvider = delegate { };
    public static Action<string> OnLobbyJoinFailed = delegate(string _reason) {  };

    public ServerProviderClient(string _serverIp, int _serverPort)
    {
        _ServerIp = _serverIp;
        _ServerPort = _serverPort;
        
        ConnectClient();
    }

    public static void ConnectClient()
    { 
        //create new client if it's null
        if (_Client == null)
        {
            _Client = new WatsonTcpClient(_ServerIp, _ServerPort);
            
            _Client.Events.ServerConnected += ServerConnected;
            _Client.Events.ServerDisconnected += ServerDisconnected;
            _Client.Events.MessageReceived += MessageReceived;
        
            _Client.Settings.DebugMessages = _DebugMessages;
            _Client.Settings.Logger = Logger;
            _Client.Settings.NoDelay = true;

            _Client.Keepalive.EnableTcpKeepAlives = true;
            _Client.Keepalive.TcpKeepAliveInterval = 1;
            _Client.Keepalive.TcpKeepAliveTime = 1;
            _Client.Keepalive.TcpKeepAliveRetryCount = 3;   
        }
        
        _Client.Connect();
    }

    public static void DisconnectClient()
    {
        if(_Client.Connected) _Client.Disconnect();
    }

    public static void SendMessage(string _message, Dictionary<object, object> _metadata)
    {
        _Client.Send(_message, _metadata);
    }

    private static void MessageReceived(object sender, MessageReceivedEventArgs args)
    {
        string receivedString = (args.Data != null) ? Encoding.UTF8.GetString(args.Data) : "[null]";
        Debug.Log($"<color=#25aaef>Received message from server provider</color> ({args.IpPort}): {receivedString}");

        if (args.Metadata != null && args.Metadata.Count > 0)
        {
            Debug.Log("Metadata:");
            foreach (KeyValuePair<object, object> curr in args.Metadata)
            {
                Debug.Log("  " + curr.Key.ToString() + ": " + curr.Value.ToString());
            }
        }
        
        //-- Collab Project --
        if (receivedString == "clientCanJoin")
        {
            string portString = (string)args.Metadata["port"];
            string lobbyCode = (string)args.Metadata["lobbyCode"];

            //join Unity server
            ushort port = ushort.Parse(portString);
            UnityThread.ExecuteInUpdate( () => ServerProviderCommunication.Instance.ClientCanJoin(port, lobbyCode));
        }

        if (receivedString == "lobbyJoinFailed")
        {
            string reason = "[unknown error]";
            if(args.Metadata != null && args.Metadata.ContainsKey("reason")) reason = (string)args.Metadata["reason"];
            UnityThread.ExecuteInUpdate(()=> OnLobbyJoinFailed?.Invoke(reason));
        }
    }

    private static void ServerConnected(object sender, ConnectionEventArgs args) 
    {
        Debug.Log(args.IpPort + " connected"); 
    }

    private static void ServerDisconnected(object sender, DisconnectionEventArgs args)
    {
        Debug.Log(args.IpPort + " disconnected: " + args.Reason.ToString());
    }

    private static void Logger(Severity sev, string msg)
    {
        //don't log debug severity
        if (sev is Severity.Debug) return;
        
        if (sev is Severity.Info)
            Debug.Log(msg);

        else if (sev is Severity.Warn or Severity.Alert)
            Debug.LogWarning(msg);

        else if(sev is Severity.Critical or Severity.Emergency or Severity.Error)
            Debug.LogError(msg);
    }
}