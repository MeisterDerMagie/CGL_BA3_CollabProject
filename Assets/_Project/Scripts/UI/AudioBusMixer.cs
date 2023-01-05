using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBusMixer : MonoBehaviour
{
    FMOD.Studio.Bus bus;
    [Tooltip("Music for Backing Track and Instruments for Player Instruments")]
    [SerializeField] private string busName;

    void Start()
    {
        bus = FMODUnity.RuntimeManager.GetBus("bus:/" + busName);
        SetLevel(PlayerPrefs.GetFloat(busName, 1f));
        GetComponent<UnityEngine.UI.Slider>().value = PlayerPrefs.GetFloat(busName, 1f);
    }

    public void SetLevel (float newVolume)
    {
        bus.setVolume(newVolume);
        PlayerPrefs.SetFloat(busName, newVolume);
    }
}
