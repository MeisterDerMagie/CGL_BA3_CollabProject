//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wichtel.Animation;

public class HitFeedbackIcon : HitFeedbackAnimations
{
    [SerializeField] private SpriteRenderer playerIcon;

    public void SetPlayerIcon(uint characterId)
    {
        Sprite icon = (characterId == uint.MaxValue) ? null : CharacterManager.Instance.GetCharacter(characterId).characterIcon;
        playerIcon.sprite = icon;
    }
}