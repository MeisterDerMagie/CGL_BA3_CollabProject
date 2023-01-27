using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wichtel.Extensions;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private RectTransform chatWindow;
    [SerializeField] private Image scrim;
    [SerializeField] private RectTransform newMessagePopup, newMessagePopupContainer;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private Ease ease;
    
    private bool _isUnfolded = false;
    private Tween _chatWindowTween;
    private Tween _scrimTween;
    private float _scrimDefaultAlpha;
    private ChatSounds sounds;
    
    private void Start()
    {
        scrim.gameObject.SetActive(true);
        _scrimDefaultAlpha = scrim.color.a;
        scrim.color = scrim.color.With(a: 0f);
        scrim.raycastTarget = false;
        sounds = GetComponent<ChatSounds>();
    }

    public void ToggleFold()
    {
        if(_isUnfolded)
            Fold();
        else
            Unfold();
    }

    private void Unfold()
    {
        _chatWindowTween?.Kill();
        _scrimTween?.Kill();

        //activate inputField
        inputField.interactable = true;
        
        //focus inputField
        inputField.Select();
        inputField.ActivateInputField();

        //activate scrim
        scrim.raycastTarget = true;
        _scrimTween = scrim.DOFade(_scrimDefaultAlpha, animDuration/2f).SetEase(Ease.Linear);

        //slide in chatWindow
        _chatWindowTween = chatWindow.DOAnchorPosX(0f, animDuration).SetEase(ease);
        
        //deactivate NewMessagePopup
        newMessagePopupContainer.gameObject.SetActive(false);
        newMessagePopup.gameObject.SetActive(false);

        _isUnfolded = true;
        if (sounds != null) sounds.PlayOpen();
    }

    private void Fold()
    {
        _chatWindowTween?.Kill();
        _scrimTween?.Kill();
        
        //deactivate inputField
        inputField.interactable = false;

        //deactivate scrim
        //scrim.gameObject.SetActive(false);
        scrim.raycastTarget = false;
        _scrimTween = scrim.DOFade(0f, animDuration/2f).SetEase(Ease.Linear);

        //slide out chatWindow
        _chatWindowTween = chatWindow.DOAnchorPosX(chatWindow.sizeDelta.x, animDuration).SetEase(ease);
        
        //activate NewMessagePopup
        newMessagePopupContainer.gameObject.SetActive(true);
        newMessagePopup.gameObject.SetActive(false);
        
        _isUnfolded = false;
        if (sounds != null) sounds.PlayClose();
    }

    public void PlayNewMessageSound()
    {
        if (!_isUnfolded)
            if (sounds != null) sounds.PlayMessage();
    }
}
