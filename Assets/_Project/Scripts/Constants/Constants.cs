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

    //Singleton
    #region Singleton
    private static Constants _instance;
    public static Constants instance => (_instance == null) ? GetOrCreateInstance() : _instance;

    private static Constants GetOrCreateInstance()
    {
        var fileName = "Constants";
        var assetPath = $"Assets/Resources/{fileName}.asset";

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
}