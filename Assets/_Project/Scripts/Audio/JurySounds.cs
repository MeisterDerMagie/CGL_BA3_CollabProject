using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class JurySounds : MonoBehaviour
{
    [SerializeField] private EventReference crowd;
    private FMOD.Studio.EventInstance crowdInstance;
    [SerializeField] private EventReference drumroll;
    [SerializeField] private EventReference jurySchild;
    [SerializeField] private EventReference applauseLo;
    [SerializeField] private EventReference applauseMid;
    [SerializeField] private EventReference applauseHi;

    // Start is called before the first frame update
    void Start()
    {
        crowdInstance = RuntimeManager.CreateInstance(crowd);
        crowdInstance.start();

        RuntimeManager.PlayOneShot(drumroll);
    }

    public void PlayApplause()
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        int maxPoints = Constants.MAX_POINTS_PERFORMANCE + Constants.MAX_POINTS_PLAYABILITY;
        float third = maxPoints / 3f;
        float twothirds = 2f* maxPoints / 3f;
        if (PlayerData.LocalPlayerData.TotalPoints < third)
        {
            RuntimeManager.PlayOneShot(applauseLo);
        }
        else if (PlayerData.LocalPlayerData.TotalPoints < twothirds)
        {
            RuntimeManager.PlayOneShot(applauseMid);
        }
        else
        {
            RuntimeManager.PlayOneShot(applauseHi);
        }
    }

    public void PlaySchild()
    {
        RuntimeManager.PlayOneShot(jurySchild);
    }

    private void OnDestroy()
    {
        crowdInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        crowdInstance.release();
    }
}
