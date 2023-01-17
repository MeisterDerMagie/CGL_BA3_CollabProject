using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    [SerializeField] private List<Light> lights = new();
    
    private List<Light> _forcedLights = new();

    //ignores mouse over
    public void ForceLightPower(int index, float power)
    {
        Light currentLight = lights[index];
        
        if (!_forcedLights.Contains(currentLight)) _forcedLights.Add(currentLight);
        currentLight.SetPower(power);
    }

    //makes a light available for mouse over again (and turns it off)
    public void UnforceLightPower(int index)
    {
        Light currentLight = lights[index];

        if (_forcedLights.Contains(currentLight)) _forcedLights.Remove(currentLight);
        currentLight.SetPower(0f);
    }

    public void MouseEnter(int index)
    {
        Light currentLight = lights[index];
        if(!_forcedLights.Contains(currentLight)) currentLight.SetPower(0.5f);
    }

    public void MouseExit(int index)
    {
        Light currentLight = lights[index];
        if (!_forcedLights.Contains(currentLight)) currentLight.SetPower(0f);
    }
}
