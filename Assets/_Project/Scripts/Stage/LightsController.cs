using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    [SerializeField] private List<Light> lights = new();
    [SerializeField] private LightsMode mode = LightsMode.HighlightMouseOverPlayer;
    [SerializeField] private float mouseOverPower = 0.5f;

    private List<Light> _forcedLights = new();


    //ignores mouse over
    public void ForceLightPower(int index, float power)
    {
        Light light = lights[index];
        
        if (!_forcedLights.Contains(light)) _forcedLights.Add(light);
        light.SetPower(power);
    }

    //makes a light available for mouse over again (and turns it off)
    public void UnforceLightPower(int index)
    {
        Light light = lights[index];

        if (_forcedLights.Contains(light)) _forcedLights.Remove(light);
        light.SetPower(0f);
    }

    public void MouseEnter(int index)
    {
        Light light = lights[index];
        if(!_forcedLights.Contains(light)) light.SetPower(0.5f);
    }

    public void MouseExit(int index)
    {
        Light light = lights[index];
        if (!_forcedLights.Contains(light)) light.SetPower(0f);
    }

    public enum LightsMode
    {
        NONE,
        HighlightMouseOverPlayer,
        Wander
    }
}
