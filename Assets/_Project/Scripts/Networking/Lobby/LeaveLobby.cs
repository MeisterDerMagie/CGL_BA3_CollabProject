//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaveLobby : MonoBehaviour
{
    public void Leave() => NetworkManager.Singleton.Shutdown();
}