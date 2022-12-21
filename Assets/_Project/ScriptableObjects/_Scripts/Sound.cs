//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "One Bar Wonder/Sound", order = 0)]
public class Sound : ScriptableObject
{
    public uint soundId;
    public EventReference soundEvent;
    [PreviewField] public Sprite soundIcon;
}