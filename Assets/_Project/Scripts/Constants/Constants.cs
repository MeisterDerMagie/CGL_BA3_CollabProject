//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Wichtel;

// ReSharper disable InconsistentNaming

public class Constants : ScriptableObject
{
    //Constants
    public static float INSTRUMENTS_AMOUNT => instance._INSTRUMENTS_AMOUNT;
    [Title("Game Constants")]
    [SerializeField] private float _INSTRUMENTS_AMOUNT;

    public static Color ownPlayerNameColor => instance._ownPlayerNameColor;
    [SerializeField] private Color _ownPlayerNameColor;
    
    public static int MIN_PLAYER_AMOUNT => instance._MIN_PLAYER_AMOUNT;
    [SerializeField] private int _MIN_PLAYER_AMOUNT;
    
    public static int RECORDING_LENGTH => instance._RECORDING_LENGTH;
    [SerializeField] private int _RECORDING_LENGTH;
    
    [Title("Scoring")]
    public static int POINTS_CREATIVITY_FIRST_VOTE => instance._POINTS_CREATIVITY_FIRST_VOTE;
    [SerializeField] private int _POINTS_CREATIVITY_FIRST_VOTE;
    
    public static int POINTS_CREATIVITY_SECOND_VOTE => instance._POINTS_CREATIVITY_SECOND_VOTE;
    [SerializeField] private int _POINTS_CREATIVITY_SECOND_VOTE;
    
    public static int POINTS_CREATIVITY_THIRD_VOTE => instance._POINTS_CREATIVITY_THIRD_VOTE;
    [SerializeField] private int _POINTS_CREATIVITY_THIRD_VOTE;
    
    public static int MAX_POINTS_PERFORMANCE => instance._MAX_POINTS_PERFORMANCE;
    [SerializeField] private int _MAX_POINTS_PERFORMANCE;
    
    public static int MAX_POINTS_PLAYABILITY => instance._MAX_POINTS_PLAYABILITY;
    [SerializeField] private int _MAX_POINTS_PLAYABILITY;
    
    //Singleton
    #region Singleton
    private static Constants _instance;
    public static Constants instance => (_instance == null) ? GetOrCreateInstance() : _instance;

    private static Constants GetOrCreateInstance()
    {
        string fileName = "Constants";
        string assetPath = $"Assets/Resources/{fileName}.asset";

        var constantsScriptableObject = Resources.Load<Constants>(fileName);

        if (constantsScriptableObject != null)
        {
            _instance = constantsScriptableObject; //SO exist
            return _instance;
        }
        
        #if UNITY_EDITOR
        //create SO if it doesn't exist
        constantsScriptableObject = CreateInstance<Constants>();
        string resourcesFullPath = AssetDatabaseUtilities.AssetPathToFullPath("Assets/Resources");
        if(!Directory.Exists(resourcesFullPath))
            Directory.CreateDirectory(resourcesFullPath);
        AssetDatabase.CreateAsset(constantsScriptableObject, assetPath);

        Debug.Log("Created constants in Resources folder.");

        _instance = constantsScriptableObject;
        #endif
        
        if(_instance == null) Debug.LogError("Could not load constants!");
        
        return _instance;
    }
    #endregion
    
    //Edit button
    #region Edit Button
    #if UNITY_EDITOR
    [Button, Title(" ")]
    private void Edit() => AssetDatabase.OpenAsset(MonoScript.FromScriptableObject(this));
    #endif
    #endregion
}