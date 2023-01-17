using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyScoring : MonoBehaviour
{
    [SerializeField] float pointForHit = 25f;
    [SerializeField] float pointForAlmost = 10f;
    [SerializeField] float pointForMiss = 0f;

    [Space]
    public float maxPointsAccuracy;
    public List<float> maxPointsPlayability;

    public float playerPoints;
    public List<float> playabilityPoints;

    public void SetUpTestScoring(List<BeatMapping.ScoringNote> notes)
    {
        maxPointsPlayability = new List<float>();
        playabilityPoints = new List<float>();

        // get max possible points
    }

    public void Score(BeatMapping.ScoringType type)
    {

    }

    public void ScorePlayability(BeatMapping.ScoringType type, int player)
    {

    }

    public void SendToServer()
    {

    }
}
