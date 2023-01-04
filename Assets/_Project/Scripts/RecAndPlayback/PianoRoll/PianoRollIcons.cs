using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PianoRollIcons : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _renderers;

    public void SetUpIcons()
    {
        if (NetworkManager.Singleton.IsServer) return;

        for (int i = 0; i < _renderers.Count; i++)
        {
            int ID = PlayerData.LocalPlayerData.InstrumentIds[i];
            Sprite sprite = InstrumentsManager.Instance.GetInstrument(ID).instrumentIcon;
            _renderers[i].sprite = sprite;
        }
    }
}
