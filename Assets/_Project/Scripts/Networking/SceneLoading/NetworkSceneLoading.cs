//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneLoading : MonoBehaviour
{
    private static readonly Dictionary<string /*sceneName*/, bool /*loadIsComplete*/> CurrentlyLoadingScenes = new Dictionary<string, bool>();

    private bool _isInitialized;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            _isInitialized = false;
            return;
        }

        if (_isInitialized) return;
        
        //do the initialization in Update because the NetworkSceneManager only exists after the server or client is started
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;

        _isInitialized = true;
    }

    private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        //this scene has finished loading on all clients
        CurrentlyLoadingScenes[sceneName] = true;
    }

    public static void LoadNetworkScene(NetworkSceneLoader sceneLoader, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("This method must be only called on the server!");
            return;
        }

        Timing.RunCoroutine(_LoadNetworkScene(sceneLoader, loadSceneMode));
    }

    private static IEnumerator<float> _LoadNetworkScene(NetworkSceneLoader sceneLoader, LoadSceneMode loadSceneMode)
    {
        //show loading screen
        GameObject loadingScreenInstance = null;
        LoadingScreen loadingScreenScript = null;
        if (sceneLoader.loadingScreenPrefab != null)
        {
            loadingScreenInstance = Instantiate(sceneLoader.loadingScreenPrefab.gameObject, Vector3.zero, Quaternion.identity);
            loadingScreenInstance.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
            loadingScreenScript = loadingScreenInstance.GetComponent<LoadingScreen>();

            //wait until the loading screens in-animation is done
            yield return Timing.WaitForSeconds(loadingScreenScript.InAnimDuration);
        }
        
        //remember the scene we are loading so that we can wait until it's done
        CurrentlyLoadingScenes[sceneLoader.scene] = false;
        
        //load scene
        NetworkManager.Singleton.SceneManager.LoadScene(sceneLoader.scene, loadSceneMode);
        
        //if we didn't have a loading screen, we are done here
        if (loadingScreenInstance == null)
        {
            if (CurrentlyLoadingScenes.ContainsKey(sceneLoader.scene)) CurrentlyLoadingScenes.Remove(sceneLoader.scene);
            yield break;
        }
        
        //otherwise, wait until every client has loaded and is synced, then hide the loading screen
        Timing.WaitUntilTrue(()=> CurrentlyLoadingScenes[sceneLoader.scene]);
        
        //remove the loaded scene the dictionary
        if (CurrentlyLoadingScenes.ContainsKey(sceneLoader.scene)) CurrentlyLoadingScenes.Remove(sceneLoader.scene);

       //play outAnimation of the loading screen on clients (on the server the out anim will not be played, but that's not an issue)
        loadingScreenScript.PlayOutAnimationClientRpc();
        
        //wait until the loading screens out-animation is done
        yield return Timing.WaitForSeconds(loadingScreenScript.OutAnimDuration);
        
        //then despawn/destroy the loading screen
        loadingScreenInstance.GetComponent<NetworkObject>().Despawn();
    }
}