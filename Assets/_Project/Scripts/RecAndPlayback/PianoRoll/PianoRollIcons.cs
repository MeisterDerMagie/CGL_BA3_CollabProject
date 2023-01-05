using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// sets up the icons at the very left of the piano roll to mark which instrument is displayed on which line
/// </summary>

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
