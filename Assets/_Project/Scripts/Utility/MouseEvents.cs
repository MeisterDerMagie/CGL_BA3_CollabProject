//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent onMouseEnter, onMouseExit, onMouseDown, onMouseUp;

    public event Action OnMouseEnterEvent = delegate {  };
    public event Action OnMouseExitEvent = delegate {  };
    public event Action OnMouseDownEvent = delegate {  };
    public event Action OnMouseUpEvent = delegate {  };
    
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
        OnMouseDownEvent?.Invoke();
        onMouseDown.Invoke();
    }

    private void OnMouseUp()
    {
        OnMouseUpEvent?.Invoke();
        onMouseUp.Invoke();
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("MouseEvents need a collider on the same gameObject to work.", this);
        }
    }
    #endif
}