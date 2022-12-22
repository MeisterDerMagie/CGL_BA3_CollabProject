//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Instrument", menuName = "One Bar Wonder/Instrument", order = 0)]
public class Instrument : ScriptableObject
{
    public uint instrumentId;
    public string friendlyName;
    public EventReference soundEvent;
    [PreviewField] public Sprite instrumentIcon;
}