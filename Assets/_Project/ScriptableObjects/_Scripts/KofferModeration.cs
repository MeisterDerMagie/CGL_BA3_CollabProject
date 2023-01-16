using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SchubiduModeration", menuName = "One Bar Wonder/SchubiduModeration", order = 0)]
public class KofferModeration : ScriptableObject
{
    [TextArea]
    public List<string> startModeration;

    [Space]
    [TextArea]
    public List<string> playback1;
    [TextArea]
    public List<string> repeat1;

    [Space]
    [TextArea]
    public List<string> playback2;
    [TextArea]
    public List<string> repeat2;

    [Space]
    [TextArea]
    public List<string> playback3;
    [TextArea]
    public List<string> repeat3;

    [Space]
    [TextArea]
    public List<string> playback4;
    [TextArea]
    public List<string> repeat4;

    [Space]
    [TextArea]
    public List<string> playback5;
    [TextArea]
    public List<string> repeat5;

    [Space]
    [TextArea]
    public List<string> playback6;
    [TextArea]
    public List<string> repeat6;

    [Space]
    [TextArea]
    public List<string> playback7;
    [TextArea]
    public List<string> repeat7;

    [Space]
    [TextArea]
    public List<string> playback8;
    [TextArea]
    public List<string> repeat8;

    [Space]
    [TextArea]
    public List<string> closingModeration;
}
