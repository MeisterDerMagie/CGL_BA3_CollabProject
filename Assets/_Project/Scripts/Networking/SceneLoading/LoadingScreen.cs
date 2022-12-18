using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Wichtel.Animation;

[RequireComponent(typeof(LoadingScreenAnimations))]
public class LoadingScreen : MonoBehaviour
{
    private AnimatorStateReference inAnim => GetComponent<LoadingScreenAnimations>().inAnim;
    private AnimatorStateReference outAnim => GetComponent<LoadingScreenAnimations>().outAnim;
    public float InAnimDuration => inAnim.Duration;
    public float OutAnimDuration => outAnim.Duration;

    private void Start()
    {
        //play in-animation
        inAnim.Play();
    }

    [ClientRpc]
    public void PlayOutAnimationClientRpc()
    {
        //play out-animation
        outAnim.Play();
    }

}
