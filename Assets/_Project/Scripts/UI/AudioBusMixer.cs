using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controls the bus level via the slider
/// saves the level in player prefs and stores that value in between scenes and playthroughs
/// name of the bus can be set in the inspector
/// </summary>

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
