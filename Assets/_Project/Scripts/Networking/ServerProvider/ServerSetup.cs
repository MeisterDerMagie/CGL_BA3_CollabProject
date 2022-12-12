//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Serialization;
using Wichtel;

//Das hier ist von Interesse für den Server
public class ServerSetup : MonoBehaviour
{
    private void Start()
    {
        #if UNITY_SERVER
        var transport = FindObjectOfType<UnityTransport>();

        Dictionary<string, string> args = CommandLineController.CommandLineArguments;
        
        //port settings
        if (args.ContainsKey("port"))
        {
            bool couldParse = ushort.TryParse(args["port"], out ushort port);
            transport.SetConnectionData(transport.ConnectionData.Address, couldParse ? port : transport.ConnectionData.Port, listenAddress: transport.ConnectionData.ServerListenAddress);

            if(couldParse) Debug.Log($"Set server port to {port}");
        }
        
        //lobbyCode
        if (args.ContainsKey("lobbyCode"))
        {
            LobbyCode.Instance.code = args["lobbyCode"];
            Debug.Log($"Set lobby code to {args["lobbyCode"]}");
        }
        
        //Start server
        NetworkManager.Singleton.StartServer();
        #endif
    }
}