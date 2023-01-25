using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyScoring : MonoBehaviour
{
    [SerializeField] private LoadNextSceneWhenAllClientsAreDone _loadNext;

    [SerializeField] float pointForHit = 25f;
    [SerializeField] float pointForAlmost = 10f;
    [SerializeField] float pointForMiss = 0f;

    [Space]
    public float maxPointsAccuracy;
    public Dictionary<Guid/*clientGuid*/, float/*playability percent*/> maxPointsPlayability = new();

    public float playerPoints;
    public Dictionary<Guid/*clientGuid*/, float/*playability percent*/> playabilityPoints = new();

    public void SetUpScoringLocal(List<BeatMapping.ScoringNote> notes, int playerAmount)
    {
        maxPointsPlayability.Clear();
        playabilityPoints.Clear();

        #region playability max points per player ermitteln
        // get the amount of notes in each player's recording:
        List<int> amountPerPlayer = new List<int>();

        // for every player, add an int to the amountperplayer list + to the playability points list
        for (int i = 0; i < playerAmount; i++)
        {
            amountPerPlayer.Add(0);
            playabilityPoints.Add(0);
        }

        // for every note in the recording list, add a note to the player whose id it is
        for (int i = 0; i < notes.Count; i++)
            amountPerPlayer[notes[i].playerID]++;

        // for every player, add to the playability points list the amount of notes of that player times the max points per note (hit points)
        for (int i = 0; i < playerAmount; i++)
            maxPointsPlayability.Add(pointForHit * amountPerPlayer[i]);
        #endregion

        #region max accuracy points ermitteln
        maxPointsAccuracy = 0;

        // for every player: add first players note amount * total amount of players, next one less, next one less, etc.
        for (int i = 0; i < playerAmount; i++)
            maxPointsAccuracy += (playerAmount - i) * amountPerPlayer[i];
        maxPointsAccuracy *= pointForHit;

        #endregion
    }

    public void SetUpScoringNetwork(List<PlayerData> players)
    {
        maxPointsPlayability.Clear();
        playabilityPoints.Clear();

        // for every player
        foreach (PlayerData player in players)
        {
            int amount = 0;

            // create a 0 for playability points
            playabilityPoints.Add(player.ClientGuid, 0);

            // go through every player's recording, count how many notes contain
            for (int note = 0; note < player.Recording.Count; note++)
                if (player.Recording[note].contains) amount++;

            // add playability points for that player
            float points = amount * pointForHit;
            maxPointsPlayability.Add(player.ClientGuid, points);

            // add to maxpoints accuracy the amount of that player
            maxPointsAccuracy += points;
        }
    }

    public void Score(BeatMapping.ScoringType type)
    {
        switch (type)
        {
            case BeatMapping.ScoringType.HIT:
                playerPoints += pointForHit;
                break;
            case BeatMapping.ScoringType.ALMOST:
                playerPoints += pointForAlmost;
                break;
            case BeatMapping.ScoringType.MISS:
                playerPoints += pointForMiss;
                break;
            default:
                break;
        }
    }

    public void ScorePlayability(BeatMapping.ScoringType type, int player)
    {
        switch (type)
        {
            case BeatMapping.ScoringType.HIT:
                playabilityPoints[player] += pointForHit;
                break;
            case BeatMapping.ScoringType.ALMOST:
                playabilityPoints[player] += pointForAlmost;
                break;
            case BeatMapping.ScoringType.MISS:
                break;
            default:
                break;
        }
    }

    public void SendToServer(bool testLocally)
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        // prozentsatz für local player accuracy ermitteln
        float accuracyPercent = (playerPoints) / maxPointsAccuracy;
        if (testLocally) Debug.Log("players accuracy was: " + accuracyPercent);

        // prozentsatz für alle spieler playability ermitteln
        List<float> playabilityPercent = new List<float>();
        for (int i = 0; i < maxPointsPlayability.Count; i++)
        {
            float playability = (playabilityPoints[i]) / maxPointsPlayability[i];
            if (testLocally) Debug.Log($"Playability for player {i + 1} was {playability}");
        }

        if (!testLocally)
        {
            // send to server
            PlayerData.LocalPlayerData.AddPointsPerformancePercent(accuracyPercent);

            for (int i = 0; i < playabilityPercent.Count; i++)
            {

            }

            // send player to next stage
            _loadNext.Done();
        }
    }
}
