using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Podium : NetworkBehaviour
{
    [SerializeField] private Transform playerPosition;

    public Vector2 PlayerPosition => playerPosition.position;
    
    private NetworkVariable<bool> _isActive = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        _isActive.OnValueChanged += OnActiveStateChanged;
        gameObject.SetActive(_isActive.Value);
    }

    public void SetActive(bool isActive)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("This method should only be called on the server.");
            return;
        }

        _isActive.Value = isActive;
        
        gameObject.SetActive(isActive);
    }
    
    private void OnActiveStateChanged(bool previousvalue, bool newvalue)
    {
        gameObject.SetActive(newvalue);
    }
}
