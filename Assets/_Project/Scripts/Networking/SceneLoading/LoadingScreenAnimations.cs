//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wichtel.Animation;

//why is this not inside the LoadingScreen class? Because then the custom Odin Inspector doesn't work.....
[RequireComponent(typeof(LoadingScreen))]
public class LoadingScreenAnimations : MonoBehaviour
{
    [SerializeField] public AnimatorStateReference inAnim, outAnim;
}