using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerLobbySounds : MonoBehaviour
{
    [SerializeField] private EventReference applause;

    void Start()
    {
        RuntimeManager.PlayOneShot(applause);
    }
}
