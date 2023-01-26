//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Wichtel;
using Random = UnityEngine.Random;

public class BackingTrackManager : MonoBehaviour
{
    #region Singleton
    private static BackingTrackManager instance;
    public static BackingTrackManager Instance => instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Debug.LogWarning("Singleton instance already exists. You should never initialize a second one.", this);
    }
    #endregion

    [SerializeField, ReadOnly] private List<BackingTrackObject> backingTracks = new List<BackingTrackObject>();

    public BackingTrackObject GetBackingTrack(int id)
    {
        foreach (BackingTrackObject backingTrack in backingTracks)
        {
            if (backingTrack.backingTrackId == id) return backingTrack;
        }
        
        Debug.LogError($"You tried to get a backingTrack that doesn't exist. Id: {id.ToString()}");
        return null;
    }

    public int GetRandomBackingTrackId()
    {
        int index = Random.Range(0, backingTracks.Count);
        return backingTracks[index].backingTrackId;
    }

    public bool IsValidBackingTrackId(int id)
    {
        foreach (BackingTrackObject backingTrack in backingTracks)
        {
            if (backingTrack.backingTrackId == id) return true;
        }

        return false;
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        //find all instruments in the Asset database
        backingTracks = EditorUtilities.GetAssets<BackingTrackObject>();
        
        //check if there any duplicate ids
        var ids = new List<int>();
        foreach (BackingTrackObject backingTrack in backingTracks)
        {
            if (!ids.Contains(backingTrack.backingTrackId))
                ids.Add(backingTrack.backingTrackId);
            else
                Debug.LogError("There are two or more backingTracks with the same ID. Make sure that each backingTrack has a different ID!");
        }
        
        //order them by their ID
        backingTracks = backingTracks.OrderBy(backingTrack => backingTrack.backingTrackId).ToList();
    }
    #endif
}