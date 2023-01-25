//(c) copyright by Martin M. Klöckener
using System;
using UnityEngine;
using Wichtel.Animation;

public class HitFeedbackAnimations : MonoBehaviour
{
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
}