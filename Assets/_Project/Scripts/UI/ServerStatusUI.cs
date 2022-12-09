using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStatusUI : MonoBehaviour
{
    [SerializeField] private Transform onlineView, offlineView;

    private void Start()
    {
        onlineView.gameObject.SetActive(false);
        offlineView.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (ServerProviderClient.Connected && !onlineView.gameObject.activeSelf)
        {
            onlineView.gameObject.SetActive(true);
            offlineView.gameObject.SetActive(false);
        }
        else if (!ServerProviderClient.Connected && !offlineView.gameObject.activeSelf)
        {
            onlineView.gameObject.SetActive(false);
            offlineView.gameObject.SetActive(true);
        }
    }
}
