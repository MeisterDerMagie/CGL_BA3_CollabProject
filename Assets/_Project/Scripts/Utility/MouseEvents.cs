//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Wichtel;

public class MouseEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent onMouseEnter, onMouseExit, onMouseDown, onMouseUp;

    public event Action OnMouseEnterEvent = delegate {  };
    public event Action OnMouseExitEvent = delegate {  };
    public event Action OnMouseDownEvent = delegate {  };
    public event Action OnMouseUpEvent = delegate {  };
    
    private void OnMouseEnter()
    {
        if (!Application.isFocused) return;
        if (UIUtilities.PointerIsOverUI()) return;
        
        OnMouseEnterEvent?.Invoke();
        onMouseEnter.Invoke();
    }

    private void OnMouseExit()
    {
        if (!Application.isFocused) return;
        if (UIUtilities.PointerIsOverUI()) return;

        OnMouseExitEvent?.Invoke();
        onMouseExit.Invoke();
    }

    private void OnMouseDown()
    {
        if (!Application.isFocused) return;
        if (UIUtilities.PointerIsOverUI()) return;

        OnMouseDownEvent?.Invoke();
        onMouseDown.Invoke();
    }

    private void OnMouseUp()
    {
        if (!Application.isFocused) return;
        if (UIUtilities.PointerIsOverUI()) return;

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