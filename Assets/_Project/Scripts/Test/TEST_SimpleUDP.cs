//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UDP;
using UnityEngine;

public class TEST_SimpleUDP : MonoBehaviour
{
    public string clientIP, serverIP;
    public ushort port;

    [SerializeField] private float offsetX, offsetY;
    private UDPSocket _server;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10 + offsetX, 300 + offsetY, 300, 300));

        if (GUILayout.Button("Start simple UDP Server"))
        {
            StartServer();
        }
        
        if (GUILayout.Button("Start simple UDP Client"))
        {
            StartClient();
        }
        
        GUILayout.EndArea();
    }

    private void StartServer()
    {
        UDPSocket s = new UDPSocket();
        s.Server(serverIP, port);
    }

    private void StartClient()
    {
        UDPSocket c = new UDPSocket();
        c.Client(clientIP, port);
        c.Send("TEST!");
    }

    private void OnDestroy()
    {
        //_server?.EndReceive();
    }
}