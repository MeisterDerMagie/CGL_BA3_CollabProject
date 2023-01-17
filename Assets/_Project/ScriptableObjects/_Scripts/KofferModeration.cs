using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SchubiduModeration", menuName = "One Bar Wonder/SchubiduModeration", order = 0)]
public class KofferModeration : ScriptableObject
{
    public List<DialogLine> startModerationLines = new();
    
    public List<DialogLine> playback1Lines = new();
    public List<DialogLine> repeat1Lines = new();
    
    public List<DialogLine> playback2Lines = new();
    public List<DialogLine> repeat2Lines = new();
    
    public List<DialogLine> playback3Lines = new();
    public List<DialogLine> repeat3Lines = new();
    
    public List<DialogLine> playback4Lines = new();
    public List<DialogLine> repeat4Lines = new();
    
    public List<DialogLine> playback5Lines = new();
    public List<DialogLine> repeat5Lines = new();
    
    public List<DialogLine> playback6Lines = new();
    public List<DialogLine> repeat6Lines = new();
    
    public List<DialogLine> playback7Lines = new();
    public List<DialogLine> repeat7Lines = new();
    
    public List<DialogLine> playback8Lines = new();
    public List<DialogLine> repeat8Lines = new();
    
    public List<DialogLine> closingModerationLines = new();
}
