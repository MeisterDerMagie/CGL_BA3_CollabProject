using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ChatSounds : MonoBehaviour
{
    [SerializeField] private EventReference open;
    [SerializeField] private EventReference close;
    [SerializeField] private EventReference msg;

    public void PlayOpen()
    {
        RuntimeManager.PlayOneShot(open);
    }

    public void PlayClose()
    {
        RuntimeManager.PlayOneShot(close);
    }

    public void PlayMessage()
    {
        RuntimeManager.PlayOneShot(msg);
    }
}
