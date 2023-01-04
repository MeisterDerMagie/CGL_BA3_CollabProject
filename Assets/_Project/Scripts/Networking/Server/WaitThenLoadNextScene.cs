using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitThenLoadNextScene : MonoBehaviour
{
    [SerializeField] private float loadDelay = 5f;
    [SerializeField] private NetworkSceneLoader sceneToLoad;
    
    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        
        Timing.RunCoroutine(_LoadSceneDelayed());
    }

    private IEnumerator<float> _LoadSceneDelayed()
    {
        yield return Timing.WaitForSeconds(loadDelay);

        NetworkSceneLoading.LoadNetworkScene(sceneToLoad, LoadSceneMode.Single);
    }
}
