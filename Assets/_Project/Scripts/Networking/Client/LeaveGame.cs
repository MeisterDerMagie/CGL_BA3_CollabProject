using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaveGame : MonoBehaviour
{
    public void Leave()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
