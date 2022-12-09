//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    //the position of the view where 0,0 means the origin (typically the mainView). 1,0 means one screen to the right of the mainView
    [SerializeField] private Vector2Int position;
}