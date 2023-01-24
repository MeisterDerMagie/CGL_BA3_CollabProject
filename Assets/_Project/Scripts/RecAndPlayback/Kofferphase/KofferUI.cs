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

    [SerializeField] private GameObject recFrame;

    void Start()
    {
        countInText.gameObject.SetActive(true);
        countInText.text = "";

        _charDisplay.SetCharacterDisplay("", PlayerData.LocalPlayerData.PlayerName, CharacterManager.Instance.GetCharacter(PlayerData.LocalPlayerData.CharacterId).characterImage);

        TurnOnLight(false);

        RecFrame(false);
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
        _charDisplay.SetDuringRR("", PlayerData.LocalPlayerData.PlayerName, 
            CharacterManager.Instance.GetCharacter(PlayerData.LocalPlayerData.CharacterId).characterImage);
    }

    public void SetDisplay(PlayerData player)
    {
        _charDisplay.SetDuringRR(player.AssignedPrompt, player.PlayerName, 
            CharacterManager.Instance.GetCharacter(player.CharacterId).characterImage);
    }

    public void Schubidu(int player, bool pb = true)
    {
        //Debug.Log("Schubidu + " + player + ", " + pb);

        if (schubiduPrefab == null) return;

        if (player == -1)
        {
            // turn off schubidu
            if (schubidu != null) schubidu.Kill();
        }
        else
        {
            // hand over correct list and tell to start
            if (player == 1)
            {
                if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback1Lines);
                else
                {
                    GameObject clone = Instantiate(schubiduPrefab);
                    schubidu = clone.GetComponent<MrSchubidu>();
                    schubidu.SetLinesAndShowNextLine(moderation.repeat1Lines);
                }
            }
            else
            {
                GameObject clone = Instantiate(schubiduPrefab);
                schubidu = clone.GetComponent<MrSchubidu>();

                if (player == 2)
                {
                    if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback2Lines);
                    else schubidu.SetLinesAndShowNextLine(moderation.repeat2Lines);
                }
                else if (player == 3)
                {
                    if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback3Lines);
                    else schubidu.SetLinesAndShowNextLine(moderation.repeat3Lines);
                }
                else if (player == 4)
                {
                    if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback4Lines);
                    else schubidu.SetLinesAndShowNextLine(moderation.repeat4Lines);
                }
                else if (player == 5)
                {
                    if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback5Lines);
                    else schubidu.SetLinesAndShowNextLine(moderation.repeat5Lines);
                }
                else if (player == 6)
                {
                    if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback6Lines);
                    else schubidu.SetLinesAndShowNextLine(moderation.repeat6Lines);
                }
                else if (player == 7)
                {
                    if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback7Lines);
                    else schubidu.SetLinesAndShowNextLine(moderation.repeat7Lines);
                }
                else if (player == 8)
                {
                    if (pb == true) schubidu.SetLinesAndShowNextLine(moderation.playback8Lines);
                    else schubidu.SetLinesAndShowNextLine(moderation.repeat8Lines);
                }
                else if (player == 9) // we have max of 8 players, so 9 means closing moderation
                {
                    schubidu.SetLinesAndShowNextLine(moderation.closingModerationLines);
                }
                else if (player == 0) // starting moderation
                {
                    schubidu.SetLinesAndShowNextLine(moderation.startModerationLines);
                }
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

    public void RecFrame(bool value)
    {
        recFrame.SetActive(value);
    }
}
