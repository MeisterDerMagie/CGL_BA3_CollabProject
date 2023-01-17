//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Wichtel.Extensions;

public class CopyLobbyCodeToClipboard : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation doTweenAnimation;
    
    public void Copy()
    {
        LobbyCode.Instance.code.CopyToClipboard();
        doTweenAnimation.DORestart();
    }
}