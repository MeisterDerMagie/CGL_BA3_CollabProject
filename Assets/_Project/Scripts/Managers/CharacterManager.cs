//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Wichtel;

public class CharacterManager : MonoBehaviour
{
    #region Singleton
    private static CharacterManager instance;
    public static CharacterManager Instance => instance;
    
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

    [SerializeField, ReadOnly] private List<Character> characters = new List<Character>();

    public Character GetCharacter(uint id)
    {
        foreach (Character character in characters)
        {
            if (character.characterId == id) return character;
        }

        Debug.LogError($"You tried to get a character that doesn't exist. Id: {id.ToString()}");
        return null;
    }

    public Character GetNextCharacter(uint id)
    {
        //if the given id was the last character, return the first one
        if ((int)(id + 1) > characters.Count - 1)
            return characters[0];

        //otherwise return the character that comes next in the list
        return characters[(int)(id + 1)];
    }

    public Character GetPreviousCharacter(uint id)
    {
        //if the given id was the first character, return the last one
        if ((int)(id - 1) < 0)
            return characters[characters.Count - 1];

        //otherwise return the character that is previous in the list
        return characters[(int)(id - 1)];
    }

    public uint GetNextId(uint id)
    {
        foreach (Character character in characters)
        {
            if (character.characterId == id) return GetNextCharacter(character.characterId).characterId;
        }
        
        Debug.LogError($"You tried to get the next characterId of an ID that doesn't exist. Id: {id.ToString()}");
        return 0;
    }

    public uint GetPreviousId(uint id)
    {
        foreach (Character character in characters)
        {
            if (character.characterId == id) return GetPreviousCharacter(character.characterId).characterId;
        }
        
        Debug.LogError($"You tried to get the previous characterId of an ID that doesn't exist. Id: {id.ToString()}");
        return 0;
    }
    
    public bool IsValidCharacterId(uint id)
    {
        foreach (Character character in characters)
        {
            if (character.characterId == id) return true;
        }

        return false;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        //find all characters in the Asset database
        characters = EditorUtilities.GetAssets<Character>();
        
        //check if there any duplicate ids
        var ids = new List<uint>();
        foreach (Character character in characters)
        {
            if (!ids.Contains(character.characterId))
                ids.Add(character.characterId);
            else
                Debug.LogError("There are two or more characters with the same ID. Make sure that each character has a different ID!");
        }
        
        //order them by their ID
        characters = characters.OrderBy(character => character.characterId).ToList();
    }
    #endif
}