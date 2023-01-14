//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wichtel.Extensions;

public class GameObjectFollowMouse : MonoBehaviour
{
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        transform.position = mouseWorldPosition.With(z: transform.position.z);
    }
}