//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NetworkSceneLoader", menuName = "One Bar Wonder/Network Scene Loader", order = 0)]
public class NetworkSceneLoader : ScriptableObject
{
    public SceneReference scene;
    public LoadingScreen loadingScreenPrefab;
}