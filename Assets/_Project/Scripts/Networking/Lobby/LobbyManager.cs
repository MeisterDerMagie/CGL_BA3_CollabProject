//(c) copyright by Martin M. Klöckener
using Sirenix.OdinInspector;
using UnityEngine;
using Wichtel;

public class LobbyManager : MonoBehaviour
{
    [DisplayAsString] public string lobbyCode;
    
    #region Singleton
    private static LobbyManager instance;
    public static LobbyManager Instance => instance;
    
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
}
