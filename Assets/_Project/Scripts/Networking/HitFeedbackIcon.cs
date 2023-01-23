//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wichtel.Animation;

public class HitFeedbackIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerIcon;
    [SerializeField] private AnimatorStateReference hitFeedback, almostFeedback, missFeedback;
    
    public void ShowFeedback(BeatMapping.ScoringType scoringType)
    {
        switch (scoringType)
        {
            case BeatMapping.ScoringType.HIT:
                hitFeedback.Play();
                break;
            case BeatMapping.ScoringType.ALMOST:
                almostFeedback.Play();
                break;
            case BeatMapping.ScoringType.MISS:
                missFeedback.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scoringType), scoringType, null);
        }
    }

    public void SetPlayerIcon(uint characterId)
    {
        Sprite icon = (characterId == uint.MaxValue) ? null : CharacterManager.Instance.GetCharacter(characterId).characterIcon;
        playerIcon.sprite = icon;
    }
}