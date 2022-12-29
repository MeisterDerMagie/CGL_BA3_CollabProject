using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Wichtel.Extensions;

[ExecuteInEditMode]
public class Light : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightOnSprite;
    [SerializeField] private SpriteMask particlesMask;
    [SerializeField] private Transform pivot, target;
    [SerializeField] private float offset;
    [SerializeField, OnValueChanged("OnPowerChangedInInspector"), Range(0f, 1f)] private float power;

    public void SetPower(float newPower)
    {
        //only numbers between 0 and 1 are allowed
        if (newPower is < 0f or > 1f)
        {
            Debug.LogWarning($"Only a value between 0 and 1 is allowed for setting the power of a light. SetPower() received a value of {newPower.ToString(CultureInfo.InvariantCulture)}.", this);
            newPower = Mathf.Clamp(newPower, 0f, 1f);
        }

        power = newPower;
        lightOnSprite.color = lightOnSprite.color.With(a: power);
        
        //set particles invisible when the power is 0
        particlesMask.gameObject.SetActive(power > 0f);
    }
    
    public void TurnOn()
    {
        SetPower(1f);
    }

    public void TurnOff()
    {
        SetPower(0f);
    }

    public void SetTargetPosition(Vector2 newPosition)
    {
        target.position = newPosition;
    }

    private void Update()
    {
        //look at target
        pivot.LookAt2D(target, offset);
    }

    #if UNITY_EDITOR
    [UsedImplicitly]
    private void OnPowerChangedInInspector()
    {
        SetPower(power);
    }
    #endif
}
