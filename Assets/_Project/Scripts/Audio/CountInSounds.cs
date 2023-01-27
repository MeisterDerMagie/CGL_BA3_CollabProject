using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CountInSounds : MonoBehaviour
{
    [Tooltip ("order: 1, 2, 3, 4: 0 is 1")]
    [SerializeField] private List<EventReference> countInSounds;

    public void PlayCountIn(int number)
    {
        RuntimeManager.PlayOneShot(countInSounds[number - 1]);
    }
}
