//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseEventsPodium : MonoBehaviour
{
    [SerializeField] private UnityEvent<int/*podiumIndex*/> onMouseEnter, onMouseExit, onMouseDown, onMouseUp;

    public event Action<int/*podiumIndex*/> OnMouseEnterEvent = delegate {  };
    public event Action<int/*podiumIndex*/> OnMouseExitEvent = delegate {  };
    public event Action<int/*podiumIndex*/> OnMouseDownEvent = delegate {  };
    public event Action<int/*podiumIndex*/> OnMouseUpEvent = delegate {  };

    private void OnMouseEnter()
    {
        if (!Application.isFocused) return;
        OnMouseEnterEvent?.Invoke(transform.GetSiblingIndex());
        onMouseEnter.Invoke(transform.GetSiblingIndex());
    }

    private void OnMouseExit()
    {
        if (!Application.isFocused) return;
        OnMouseExitEvent?.Invoke(transform.GetSiblingIndex());
        onMouseExit.Invoke(transform.GetSiblingIndex());
    }

    private void OnMouseDown()
    {
        if (!Application.isFocused) return;
        OnMouseDownEvent?.Invoke(transform.GetSiblingIndex());
        onMouseDown.Invoke(transform.GetSiblingIndex());
    }

    private void OnMouseUp()
    {
        if (!Application.isFocused) return;
        OnMouseUpEvent?.Invoke(transform.GetSiblingIndex());
        onMouseUp.Invoke(transform.GetSiblingIndex());
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