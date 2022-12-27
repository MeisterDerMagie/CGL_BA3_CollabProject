//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick, onMouseEnter, onMouseExit;

    public event Action OnClickEvent = delegate {  };
    public event Action OnMouseEnterEvent = delegate {  };
    public event Action OnMouseExitEvent = delegate {  };
    
    private void OnMouseEnter()
    {
        OnMouseEnterEvent?.Invoke();
        onMouseEnter.Invoke();
    }

    private void OnMouseExit()
    {
        OnMouseExitEvent?.Invoke();
        onMouseExit.Invoke();
    }

    private void OnMouseDown()
    {
        OnClickEvent?.Invoke();
        onClick.Invoke();
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("A Selectable needs a collider on itself to work.", this);
        }
    }
    #endif
}