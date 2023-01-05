using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sets up the display for the different characters during the playback stage
/// sets the current player name, their image and the assigned prompt so that they're visible while their bar is played back
/// also turns on/off light on the player --> during the bar the light is turned on
/// </summary>

public class CharDisplayPB : MonoBehaviour
{
    [SerializeField] private GameObject lightOn;
    [SerializeField] private GameObject lightOff;

    [SerializeField] private TMPro.TextMeshProUGUI prompt;
    [SerializeField] private TMPro.TextMeshProUGUI playerName;
    [SerializeField] private SpriteRenderer playerImage;

    public void SwitchLight(bool value)
    {
        // turn light on and off depending on value
        lightOn.SetActive(value);
        lightOff.SetActive(!value);
    }

    public void SetCharacterDisplay(string _prompt, string name, Sprite img)
    {
        // set prompt
        prompt.gameObject.SetActive(true);
        prompt.text = _prompt;
        // set player name
        playerName.gameObject.SetActive(true);
        playerName.text = name;
        // set player icon
        playerImage.gameObject.SetActive(true);
        playerImage.sprite = img;
    }

    public void TurnOffCharacter()
    {
        prompt.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        playerImage.gameObject.SetActive(false);
    }
}
