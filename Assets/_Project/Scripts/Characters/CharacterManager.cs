//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    #region Singleton
    private static CharacterManager instance;
    public static CharacterManager Instance => instance;
    
    public void Awake()
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
    
    public bool IsValidCharacterId(uint id)
    {
        Debug.LogWarning("Here we should check if the characterId is actually valid and if there exists a corresponding character.");
        
        //DEBUG
        return true;
    }
}