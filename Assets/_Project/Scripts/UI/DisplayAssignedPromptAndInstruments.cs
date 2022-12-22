using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class DisplayAssignedPromptAndInstruments : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private Transform view;
    
    private void Start()
    {
        view.gameObject.SetActive(false);
        Timing.RunCoroutine(_ShowViewDelayed().CancelWith(gameObject));
    }

    private IEnumerator<float> _ShowViewDelayed()
    {
        yield return Timing.WaitForSeconds(delay);

        view.gameObject.SetActive(true);
    }
}
