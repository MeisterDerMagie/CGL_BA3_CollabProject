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
    [SerializeField] private GameObject schubiduPrefab;

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

        TurnOnLight(false);
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

    public void Schubidu(int player, bool pb = true)
    {
        Debug.Log("Schubidu + " + player + ", " + pb);

        if (schubiduPrefab == null) return;

        if (player == -1)
        {
            // turn off schubidu
            schubidu.StopTalking();
        }
        else
        {
            // set up schubidu and sent correct list of strings over
            if (schubidu != null) schubidu.StopTalking();

            // create new instance of schubidu + save schubidu script
            GameObject clone = Instantiate(schubiduPrefab);
            schubidu = clone.GetComponent<MrSchubidu>();

            // hand over correct list and tell to start
            if (player == 1)
            {
                if (pb == true) schubidu.Talk(moderation.playback1);
                else schubidu.Talk(moderation.repeat1);
            }
            else if (player == 2)
            {
                if (pb == true) schubidu.Talk(moderation.playback2);
                else schubidu.Talk(moderation.repeat2);
            }
            else if (player == 3)
            {
                if (pb == true) schubidu.Talk(moderation.playback3);
                else schubidu.Talk(moderation.repeat3);
            }
            else if (player == 4)
            {
                if (pb == true) schubidu.Talk(moderation.playback4);
                else schubidu.Talk(moderation.repeat4);
            }
            else if (player == 5)
            {
                if (pb == true) schubidu.Talk(moderation.playback5);
                else schubidu.Talk(moderation.repeat5);
            }
            else if (player == 6)
            {
                if (pb == true) schubidu.Talk(moderation.playback6);
                else schubidu.Talk(moderation.repeat6);
            }
            else if (player == 7)
            {
                if (pb == true) schubidu.Talk(moderation.playback7);
                else schubidu.Talk(moderation.repeat7);
            }
            else if (player == 8)
            {
                if (pb == true) schubidu.Talk(moderation.playback8);
                else schubidu.Talk(moderation.repeat8);
            }
            else if (player == 9) // we have max of 8 players, so 9 means closing moderation
            {
                schubidu.Talk(moderation.closingModeration);
            }
        }
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
