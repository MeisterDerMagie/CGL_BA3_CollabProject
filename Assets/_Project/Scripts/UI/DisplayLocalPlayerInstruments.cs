//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLocalPlayerInstruments : MonoBehaviour
{
    [SerializeField] private LayoutGroup parent;
    [SerializeField] private GameObject instrumentIconPrefab;
    private bool _isInitialized;

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer || PlayerData.LocalPlayerData == null) return;

        if (_isInitialized) return;

        if (PlayerData.LocalPlayerData.InstrumentIds.Count == 0) return;

        foreach (uint instrumentId in PlayerData.LocalPlayerData.InstrumentIds)
        {
            var icon = Instantiate(instrumentIconPrefab, parent.transform);
            icon.GetComponent<Image>().sprite = InstrumentsManager.Instance.GetInstrument(instrumentId).instrumentIcon;
        }

        _isInitialized = true;
    }
}