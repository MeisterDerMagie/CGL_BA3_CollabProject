using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisuals : NetworkBehaviour
{
    [SerializeField] private Transform visuals;
    [SerializeField] private SpriteRenderer characterImage;

    public NetworkVariable<bool> isVisible = new NetworkVariable<bool>();
    private PlayerData _playerData;
    
    private uint _characterIdCached = uint.MaxValue;

    public override void OnNetworkSpawn()
    {
        _playerData = GetComponent<PlayerData>();

        isVisible.OnValueChanged += OnVisibilityChanged;
        SetVisibility(isVisible.Value);
    }

    private void OnVisibilityChanged(bool previousvalue, bool newvalue)
    {
        SetVisibility(newvalue);
    }

    private void SetVisibility(bool newValue)
    {
        visuals.gameObject.SetActive(newValue);
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    /// <summary>Set the sorting order of characters. Min value: 0, max value: 7 </summary>
    /// <param name="order">value between 0 (foremost) and 7 (rearmost)</param>
    [ClientRpc]
    public void SetSortOrderClientRpc(uint order)
    {
        characterImage.sortingOrder -= (int)order;
    }

    private void Update()
    {
        if (_characterIdCached != _playerData.CharacterId)
        {
            _characterIdCached = _playerData.CharacterId;
            UpdateCharacterImage();
        } 
    }

    private void UpdateCharacterImage()
    {
        //update character image
        characterImage.sprite = CharacterManager.Instance.GetCharacter(_characterIdCached)?.characterImage;
    }
}
