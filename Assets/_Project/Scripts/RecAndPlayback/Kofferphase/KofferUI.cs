using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KofferUI : MonoBehaviour
{
    [SerializeField] private CharDisplayPB _charDisplay;
    [SerializeField] private Light _light;

    [SerializeField] private List<SpriteRenderer> djPultRenderers;
    [SerializeField] [Range(0,1)] private float greyedAlpha;

    [SerializeField] private TMPro.TextMeshProUGUI countInText;

    // reference to currently active mr schubidu
    private MrSchubidu schubidu;
    // reference to scriptable object with strings
    [SerializeField] private KofferModeration moderation;

    // liste für jedes mal das schubidu spricht --> scriptable object

    // spawn neuen schubdiu wenn er was sagt
    // speicher reference
    // talk funktion oder stop talking
    // der talk funktion geben wir ne liste an strings mit die er sagen soll

    // Start is called before the first frame update
    void Start()
    {
        countInText.gameObject.SetActive(true);
        countInText.text = "";

        //_charDisplay.SetCharacterDisplay("", PlayerData.LocalPlayerData.PlayerName);
    }

    public void GreyOutDJPult(bool greyed)
    {
        if (greyed) // then grey out the dj pult
        {
            foreach (SpriteRenderer renderer in djPultRenderers)
            {
                Color c = renderer.color;
                c.a = greyedAlpha;
                renderer.color = c;
            }
        }
        else // then stop greying out the dj pult
        {
            foreach (SpriteRenderer renderer in djPultRenderers)
            {
                Color c = renderer.color;
                c.a = 1;
                renderer.color = c;
            }
        }
    }

    public void SetDisplayToSelf()
    {
        //_charDisplay.
    }

    public void SetDisplay(int player)
    {

    }

    public void Schubidu(bool start, int i)
    {

    }

    public void CountInText(string text)
    {
        countInText.text = text;
    }

    public void PromptText(string text)
    {
        _charDisplay.SetPrompt(text);
    }

    public void TurnOnLight(bool value)
    {
        if (value == true) _light.TurnOn();
        else _light.TurnOff();
    }
}
