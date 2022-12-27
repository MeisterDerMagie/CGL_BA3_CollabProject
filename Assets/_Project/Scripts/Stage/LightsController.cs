using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    [SerializeField] private List<Light> lights = new();
    [SerializeField] private LightsMode mode = LightsMode.HighlightMouseOverPlayer;

    private void Update()
    {
        
    }

    public enum LightsMode
    {
        NONE,
        HighlightMouseOverPlayer,
        Wander
    }
}
