//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Wichtel;

public class SoundsManager : MonoBehaviour
{
    #region Singleton
    private static SoundsManager instance;
    public static SoundsManager Instance => instance;
    
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

    [SerializeField, ReadOnly] private List<Sound> sounds = new List<Sound>();

    public Sound GetSound(uint id)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.soundId == id) return sound;
        }
        
        Debug.LogError($"You tried to get a sound that doesn't exist. Id: {id.ToString()}");
        return null;
    }

    public bool IsValidSoundId(uint id)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.soundId == id) return true;
        }

        return false;
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        //find all sounds in the Asset database
        sounds = EditorUtilities.GetAssets<Sound>();
        
        //check if there any duplicate ids
        var ids = new List<uint>();
        foreach (Sound sound in sounds)
        {
            if (!ids.Contains(sound.soundId))
                ids.Add(sound.soundId);
            else
                Debug.LogError("There are two or more sounds with the same ID. Make sure that each sound has a different ID!");
        }
        
        //order them by their ID
        sounds = sounds.OrderBy(character => character.soundId).ToList();
    }
    #endif
}