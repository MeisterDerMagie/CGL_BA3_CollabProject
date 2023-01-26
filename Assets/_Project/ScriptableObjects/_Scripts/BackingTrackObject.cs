//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BackingTrack", menuName = "One Bar Wonder/BackingTrack", order = 0)]
public class BackingTrackObject : ScriptableObject
{
    public int backingTrackId;
    public string friendlyName;
    public EventReference soundEvent;
}