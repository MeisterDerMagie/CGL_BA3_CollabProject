using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Wichtel.Extensions;

[ExecuteInEditMode]
public class Light : MonoBehaviour
{
    [SerializeField] private Sprite spriteOn, spriteOff;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform pivot, target;
    [SerializeField] private float offset;
    [SerializeField, OnValueChanged("OnStateChanged")] private bool isOn;
    
    public void TurnOn()
    {
        isOn = true;
        spriteRenderer.sprite = spriteOn;
    }

    public void TurnOff()
    {
        isOn = false;
        spriteRenderer.sprite = spriteOff;
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
    private void OnStateChanged()
    {
        if(isOn) TurnOn();
        else TurnOff();
    }
    #endif
}
