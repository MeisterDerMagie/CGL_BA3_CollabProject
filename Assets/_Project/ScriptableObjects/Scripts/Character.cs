//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "One Bar Wonder/Character", order = 0)]
public class Character : ScriptableObject
{
    public uint characterId;
    public Sprite characterImage;
    public Sprite characterIcon;
}